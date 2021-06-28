#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <math.h>
#include <time.h>
#include <assert.h>

#include "../include/io.h"
#include "../include/path.h"


int ply_save(
    int serial_number,
    int width,
    int height,
    double depth_scale,
    const char *root,
    const char *depth_file_path,
    const char *color_file_path) {
  size_t depth_size = 0, color_size = 0;

  uint16_t *depth = NULL;
  uint8_t *color = NULL;

  const time_t t = time(NULL);
  char ply_path[1024];
  memset((void *)ply_path, 0, sizeof(ply_path));
  snprintf(ply_path, 1024, "%ld.ply", t);

  FILE *pf;
  if((pf = fopen(ply_path, "wb")) == NULL) {
    return EOF;
  }

  char *_path = paths_join_allocate(2, '/', (char *)root, depth_file_path);
  depth = io_read(_path, sizeof(uint16_t), &depth_size);
  memset((void *)_path, 0, sizeof(char) * strlen(_path));
  free((void *)_path);

  _path = paths_join_allocate(2, '/', (char *)root, color_file_path);
  color = io_read(_path, sizeof(uint8_t), &color_size);
  memset((void *)_path, 0, sizeof(char) * strlen(_path));
  free((void *)_path);

#ifndef NDEBUG
  fprintf(stderr, "d depth size: %ld(%p)\n", depth_size, depth);
  fprintf(stderr, "d color size: %ld(%p)\n", color_size, color);
#endif

  assert(depth != NULL);
  assert(color != NULL);


  const char *ply_header =
    "ply\n"
    "format ascii 1.0\n"
    "element vertex %d\n"
    "property float x\n"
    "property float y\n"
    "property float z\n"
    "property uchar red\n"
    "property uchar green\n"
    "property uchar blue\n"
    "end_header\n";

  const double threshold_min = 0.52f,
        threshold_max = 10.f;

  const double one_meter = 1.f / depth_scale;

  size_t vertex_count = 0;

  for(int y = 0; y < height; ++y) {
    for(int x = 0; x < width; ++x) {
      int _offset = y * width + x;
      double z = *(depth + _offset) / one_meter;

      if(z >= threshold_min &&
          z <= threshold_max) {
        ++vertex_count;
      }
    }
  }

  fprintf(pf, ply_header, vertex_count);
  for(int y = 0; y < height; ++y) {
    double _y = (height / 2.0f) - y;
    // ViewPort Y
    double y_vp = _y * 0.784;
    double y_theta = atan(y_vp / 520.f);

    for(int x = 0; x < width; ++x) {
      double _x = (width / 2.0f) - x;
      double x_vp = _x * 0.7577;
      double x_theta = atan(x_vp / 520.f);

      int cursor = y * width + x;

      double z = *(depth + cursor);
      double distance = z / one_meter;
      if(distance >= threshold_min
          && distance <= threshold_max) {
#if 1
        fprintf(pf, "%f %f %f %d %d %d\n",
            (z * tan(x_theta)) / one_meter,
            (z * tan(y_theta)) / one_meter,
            distance,
            *(color + (cursor * 3 + 0)),
            *(color + (cursor * 3 + 1)),
            *(color + (cursor * 3 + 2)));
#else
        fprintf(pf, "%f %f %f %d %d %d\n",
            _x, _y, (double)*(depth + cursor),
            *(color + (cursor * 3 + 0)),
            *(color + (cursor * 3 + 1)),
            *(color + (cursor * 3 + 2)));
#endif
      }
    }
  }
  //fprintf(stderr, "vertex %d\n", count);

  memset((void *)depth, 0, depth_size);
  free((void *)depth);

  memset((void *)color, 0, color_size);
  free((void *)color);

  fclose(pf);
  return 0;
}
