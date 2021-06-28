#ifndef _REALSENSE_META_H
#define _REALSENSE_META_H

#include <stdint.h>
#include <stddef.h>


struct realsense_meta_t {
  int32_t serial_number;
  int32_t frame_number;
  char *stream;
  char *format;
};

extern struct realsense_meta_t *metadata_get_all(const char *root, size_t *size);

extern void *metadata_free(struct realsense_meta_t *this);

#endif
