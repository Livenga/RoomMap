#ifndef _IO_H
#define _IO_H

#include <stdio.h>


extern char *io_read_bytes(
    const char *path,
    size_t *p_size);

extern void *io_read(
    const char *path,
    size_t size,
    size_t *p_size);

#endif
