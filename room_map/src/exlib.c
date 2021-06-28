#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>


/**
*/
long strntol(char *s, char **endptr, int base, size_t size) {
  char buffer[32];
  memset((void *)buffer, 0, sizeof(buffer));

  char *p = buffer;
  int8_t is_required_free = 0;

  if(size > 32) {
    is_required_free = 1;
    p = (char *)calloc(size + 1, sizeof(char));
    strncpy(p, s, size);
  }
  strncpy(p, s, size);

  long value = strtol(buffer, endptr, base);
  if(is_required_free) {
    memset((void *)p, 0, sizeof(char) * size);
    free((void *)p);
    p = NULL;
  }

  return value;
}

static int stringeq(char *s1, char *s2, size_t size) {
  return strncmp(s1, s2, size);
}
