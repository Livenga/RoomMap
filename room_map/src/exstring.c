#include <stdio.h>
#include <string.h>
#include <stdlib.h>


char *string_ncopy(const char *s, size_t size) {
  size_t len = strlen(s);

  char *p = (char *)calloc(size + 1, sizeof(char));
  if(len < size) {
    strncpy(p, s, len);
  } else {
    strncpy(p, s, size);
  }

  return p;
}
