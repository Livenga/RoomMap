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


void *io_read(
    const char *path,
    size_t size,
    size_t *p_size) {
  FILE *pf;

#ifndef NDEBUG
  fprintf(stderr, "d io_read: %s\n", path);
#endif
  if((pf = fopen(path, "rb")) == NULL) {
    return NULL;
  }

  fseek(pf, 0L, SEEK_END);
  size_t file_size = ftell(pf);
  fseek(pf, 0L, SEEK_SET);

  if(p_size != NULL) {
    *p_size = file_size;
  }


  const size_t nmemb = file_size / size;
  void *p = malloc(nmemb * size);
  memset(p, 0, nmemb * size);

  size_t cursor = 0, read_size;
  while((read_size = fread(p + cursor, size, file_size - cursor, pf)) > 0) {
    cursor += read_size;
  }

  fclose(pf);

  return p;
}
