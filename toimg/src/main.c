#include <stdio.h>
#include <string.h>
#include <stdint.h>
#include <stdlib.h>
#include <getopt.h>
#include <unistd.h>
#include <regex.h>

#include "../include/types.h"

extern int jpeg_save(
    const char *input_path,
    int size[2],
    stream_t stream);

static int *get_size(const char *str, int *sizes);
static stream_t get_stream(const char *str);


int main(
    int argc,
    char *argv[]) {
  const struct option opts[] = {
    {"size", required_argument, 0, 's'},
    {"color", no_argument, 0, 'c'},
    {"depth", no_argument, 0, 'd'},
    {"input", required_argument, 0, 'i'},
  };

  uint8_t f_color = 0, f_depth = 0;
  int opt, optidx;

  char input_path[1024];
  int sizes[2] = { 0 };

  memset((void *)input_path, 0, sizeof(input_path));
  while((opt = getopt_long(argc, argv, "s:cdi:", opts, &optidx)) > 0) {
    char *p = NULL;

    switch(opt) {
      case 's':
        p = (optarg != NULL) ? optarg : *(argv + optind);
        if(get_size(p, sizes) != NULL) {
          fprintf(stderr, "%dx%d\n", *sizes, *(sizes + 1));
        }
        break;

      case 'c':
        f_color = 1;
        break;

      case 'd':
        f_depth = 1;
        break;

      case 'i':
        p = (optarg != NULL) ? optarg : *(argv + optind);
        strncpy(input_path, p, 1024);
        break;
    }
  }

  if(strlen(input_path) == 0) {
    return EOF;
  }

  stream_t stream_type = ANY;
  if(! f_color && ! f_depth) {
    stream_type = get_stream(input_path);
  }

  if(stream_type == COLOR) {
    jpeg_save(input_path, sizes, stream_type);
  }

  return 0;
}

static int *get_size(const char *str, int *sizes) {
  regex_t regex;

  int status = regcomp(&regex, "([0-9]*)x([0-9]*)", REG_NEWLINE | REG_EXTENDED);
  if(status != 0) {
    return NULL;
  }

  regmatch_t matches[3];
  status = regexec(&regex, str, 3, matches, 0);
  if(status != 0) {
    regfree(&regex);
    return NULL;
  }

  char buffer[32];
  memset((void *)buffer, 0, sizeof(buffer));
  strncpy(buffer, str + matches[1].rm_so, matches[1].rm_eo - matches[1].rm_so);
  *sizes = strtol(buffer, NULL, 10);

  memset((void *)buffer, 0, sizeof(buffer));
  strncpy(buffer, str + matches[2].rm_so, matches[2].rm_eo - matches[2].rm_so);
  *(sizes + 1) = strtol(buffer, NULL, 10);

  regfree(&regex);
  return sizes;
}

static stream_t get_stream(const char *str) {
  regex_t regex;

  int status = regcomp(
      &regex, "[0-9]*\\.[0-9]*\\.([0-9a-zA-Z]*)-[0-9a-zA-Z]*", REG_NEWLINE | REG_EXTENDED);
  if(status != 0) {
    return ANY;
  }

  regmatch_t matches[2];
  status = regexec(&regex, str, 2, matches, 0);
  if(status != 0) {
    regfree(&regex);
    return ANY;
  }

  char buffer[32];
  memset((void *)buffer, 0, sizeof(buffer));
  strncpy(buffer, str + matches[1].rm_so, matches[1].rm_eo - matches[1].rm_so);

  regfree(&regex);

  int s = (matches + 1)->rm_so;
  size_t size = (matches + 1)->rm_eo - (matches + 1)->rm_so;

  if(strncmp(str + s, "Color", size) == 0) {
    return COLOR;
  } else if(strncmp(str + s, "Depth", size) == 0) {
    return DEPTH;
  }

  return ANY;
}
