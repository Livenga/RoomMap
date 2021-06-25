#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <stdint.h>

char *io_read_bytes(
    const char *path,
    size_t *p_size) {
  FILE *fp = NULL;

  if((fp = fopen(path, "rb")) == NULL) {
    return NULL;
  }

  fseek(fp, 0L, SEEK_END);
  long size = ftell(fp);
  if(p_size != NULL) {
    *p_size = size;
  }
  fseek(fp, 0L, SEEK_SET);

  char *buffer = (char *)calloc(size + 1, sizeof(char));
  size_t rs, cursor = 0;

  while((rs = fread((void *)(buffer + cursor), sizeof(char), size - cursor, fp)) > 0) {
    cursor += rs;
  }

  fclose(fp);

  return buffer;
}
