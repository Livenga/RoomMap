#ifndef _IO_H
#define _IO_H

#include <stdio.h>


extern char *io_read_bytes(
    const char *path,
    size_t *p_size);

#endif
