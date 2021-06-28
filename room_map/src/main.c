#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <dirent.h>
#include <getopt.h>
#include <unistd.h>
#include <regex.h>
//#define NDEBUG
#include <assert.h>

#include "../../parson/parson.h"
#include "../include/io.h"
#include "../include/path.h"
#include "../include/device_info.h"
#include "../include/realsense_meta.h"


extern long strntol(char *s, char **endptr, int base, size_t size);
extern int ply_save(
    int serial_number,
    int width,
    int height,
    double depth_scale,
    const char *root,
    const char *depth_file_path,
    const char *color_file_path);


int main(
    int argc,
    char *argv[]) {
  const struct option opts[] = {
    {"target-directory", required_argument, NULL, 't'},
    {"number", required_argument, NULL, 'n'},
    {"all", no_argument, NULL, 'a'},
  };

  if(argc == 0) {
    return -1;
  }


  int opt, opt_index;
  char *p = NULL;
  char target_directory[1024];
  int32_t number = -1;
  int16_t f_all = 0;

  memset((void *)target_directory, 0, sizeof(target_directory));
  while((opt = getopt_long(argc, argv, "t:n:a", opts, &opt_index)) > 0) {
    switch(opt) {
      case 't':
        p = (optarg != NULL) ? optarg : *(argv + optind);
        strncpy(target_directory, p, 1024);
        break;
      case 'n':
        p = (optarg != NULL) ? optarg : *(argv + optind);
        number = strtol(p, NULL, 10);
        break;
      case 'a':
        f_all = 1;
        break;
    }
  }

  fprintf(stderr, "d Target directory: %s\n", target_directory);
  char *p_device_info_path = paths_join_allocate(2, '/', target_directory, "device_info.json");
  struct device_info_t *p_dev = device_info_get(p_device_info_path);

  fprintf(stderr, "d Record device: %s(%s)\n", p_dev->name, p_dev->serial_number);


  assert(f_all || number >= 0);

  size_t nmetas = 0;
  struct realsense_meta_t *metas = metadata_get_all(target_directory, &nmetas);

  if(f_all) {
    for(int i = 0; i < nmetas; ++i) {
      struct realsense_meta_t meta = *(metas + i);
    }
  } else if(number >= 0) {
    char color_filename[256];
    char depth_filename[256];

    struct realsense_meta_t *p_color_meta = NULL, *p_depth_meta = NULL;
    for(int i = 0; i < nmetas; ++i) {
      struct realsense_meta_t *meta = (metas + i);

      if(meta->serial_number == number) {
        if(strcmp(meta->stream, "Depth") == 0) {
          p_depth_meta = meta;

          memset((void *)depth_filename, 0, sizeof(depth_filename));
          snprintf(depth_filename, 256,
              "%d.%d.0.%s-%s",
              meta->serial_number, meta->frame_number,
              meta->stream, meta->format);
        } else if(strcmp(meta->stream, "Color") == 0) {
          p_color_meta = meta;

          memset((void *)color_filename, 0, sizeof(depth_filename));
          snprintf(color_filename, 256,
              "%d.%d.0.%s-%s",
              meta->serial_number, meta->frame_number,
              meta->stream, meta->format);
        }
      }
    }

    if(strlen(depth_filename) > 0 && strlen(color_filename) > 0) {
      int32_t width = 0, height = 0;
      for(int i = 0; i < p_dev->profile_count; ++i) {
         if(strcmp((p_dev->profiles + i)->stream, p_color_meta->stream) == 0) {
           width  = (p_dev->profiles + i)->width;
           height = (p_dev->profiles + i)->height;
         }
      }

      ply_save(
          number,
          width,
          height,
          p_dev->depth_scale,
          target_directory,
          depth_filename,
          color_filename);
    }
  } else {
    // TODO:
  }

  for(int i = 0; i < nmetas; ++i) {
    metadata_free(metas + i);
  }
  free((void *)metas);

  device_info_free(p_dev);
  memset((void *)p_device_info_path, 0, strlen(p_device_info_path));
  free((void *)p_device_info_path);

  return 0;
}
