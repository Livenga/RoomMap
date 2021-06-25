#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdarg.h>


#define _BUFFER_SIZE (1024)
static char *_path_buffer[_BUFFER_SIZE];


static size_t _get_adjust_size(char *str, char separator) {
  size_t size = strlen(str);
  int i;

  for(i = size - 1; i >= 0; --i) {
    if(*(str + i) != separator) {
      break;
    }
  }

  return i + 1;
}

/**
 */
char *path_join(
    char *p1,
    char *p2) {
  size_t p1_len = _get_adjust_size(p1, '/');
  size_t p2_len = _get_adjust_size(p2, '/');

  size_t size = p1_len + p2_len + 2;
  char *p = (char *)calloc(size, sizeof(char));

  memcpy((void *)p, (const void *)p1, p1_len);
  *(p + p1_len) = '/';
  memcpy((void *)(p + p1_len + 1), (const void *)p2, p2_len);

  return p;
}


/**
 */
char *paths_join_allocate(size_t argc, char separator, char *p1, ...) {
  va_list vars;
  size_t size = 0;

  va_start(vars, p1);

  size = _get_adjust_size(p1, separator) + 1;
  for(int i = 0; i < argc - 1; ++i) {
    char *arg = va_arg(vars, char *);
    if(arg == NULL) {
      break;
    }

    size += _get_adjust_size(arg, separator) + 1;
  }
  --size;
  va_end(vars);

  char *p = (char *)calloc(size + 1, sizeof(char));
  va_start(vars, p1);

  //strncpy(p, p1, size);
  memcpy(p, p1, _get_adjust_size(p1, separator));
  *(p + strlen(p)) = separator;

  for(int i = 0; i < argc - 1; ++i) {
    char *arg = va_arg(vars, char *);
    if(arg == NULL) {
      break;
    }

    size_t _cursor = strlen(p),
           _size = _get_adjust_size(arg, separator);
    //strncat(p, arg, size);
    memcpy((void *)(p + _cursor), arg, _size);
    *(p + strlen(p)) = separator;
  }
  *(p + strlen(p) - 1) = '\0';
  va_end(vars);

  return p;
}
