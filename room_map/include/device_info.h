#ifndef _DEVICE_INFO_H
#define _DEVICE_INFO_H

#include <stdint.h>
#include <stddef.h>


struct device_info_t {
  char *name;
  char *firmware_version;
  char *product_id;
  float depth_scale;

  size_t profile_count;
  struct profile_t *profiles;
};

struct profile_t {
  int32_t width;
  int32_t height;
  int16_t framerate;
  char    *stream;
  char    *format;
};


extern struct device_info_t *device_info_get(const char *path);

extern void device_info_free(struct device_info_t *this);

#endif
