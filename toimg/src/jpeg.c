#define NDEBUG
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdint.h>
#include <unistd.h>
#include <time.h>
#include <jpeglib.h>

#include "../include/types.h"


static void *io_read(
    const char *path,
    size_t size,
    size_t nmemb);


int jpeg_save(
    const char *input_path,
    int size[2],
    stream_t stream) {
  void *buffer = NULL;
  struct jpeg_compress_struct cinfo;
  struct jpeg_error_mgr jerr;

  cinfo.err = jpeg_std_error(&jerr);
  jpeg_create_compress(&cinfo);


  char output_path[1024];
  memset((void *)output_path, 0, sizeof(output_path));

  snprintf(output_path, 1024, "%ld.jpg", time(NULL));
  FILE *pf;
  if((pf = fopen(output_path, "wb")) == NULL) {
    return EOF;
  }

  size_t _size = stream == COLOR ? sizeof(uint8_t) : sizeof(uint16_t);
  size_t _nmemb = size[0] * size[1] * (stream == COLOR ? 3 : 1);
  buffer = io_read(input_path, _size, _nmemb);
  jpeg_stdio_dest(&cinfo, pf);

  cinfo.image_width = *size;
  cinfo.image_height = *(size + 1);
  //fprintf(stderr, "%dx%d\n", cinfo.image_width, cinfo.image_height);
  cinfo.input_components = stream == COLOR ? 3 : 1;
  cinfo.in_color_space = stream == COLOR ? JCS_RGB : JCS_GRAYSCALE;

  jpeg_set_defaults(&cinfo);
  jpeg_set_quality(&cinfo, 95, TRUE);
  jpeg_start_compress(&cinfo, TRUE);

  size_t stride = *size * (stream == COLOR ? 3 : 1);
  uint8_t *line = (uint8_t *)calloc(stride, sizeof(uint8_t));
  for(int i = 0; i < *(size + 1); ++i) {
    memcpy((void *)line, (const void *)buffer + (stride * i), stride);
    jpeg_write_scanlines(&cinfo, &line, 1);
  }
  memset((void *)line, 0, stride);
  free((void *)line);

  memset((void *)buffer, 0, _size * _nmemb);
  free((void *)buffer);


  jpeg_finish_compress(&cinfo);
  fclose(pf);
  jpeg_destroy_compress(&cinfo);
  return 0;
}

static void *io_read(
    const char *path,
    size_t size,
    size_t nmemb) {
  FILE *pf;
  if((pf = fopen(path, "rb")) == NULL) {
    return NULL;
  }

  void *buffer = malloc(size * nmemb);
  memset(buffer, 0, size * nmemb);

  size_t cursor = 0, rsize = 0;
  while((rsize = fread(buffer + cursor, size, nmemb - cursor, pf)) > 0) {
    cursor += rsize;
  }

  fclose(pf);

  return buffer;
}
