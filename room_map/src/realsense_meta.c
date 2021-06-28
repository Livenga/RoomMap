#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <dirent.h>
#include <regex.h>
#include "../include/realsense_meta.h"
#include "../include/exstring.h"


#define META_FILE_NAME_FORMAT ("([0-9]*)\\.([0-9]*)\\.[0-9].*\\.([a-zA-Z0-9].*)-([a-zA-Z0-9].*)")

extern long strntol(char *s, char **endptr, int base, size_t size);


struct realsense_meta_t *metadata_get_all(const char *root, size_t *p_nmetas) {
  int rcode = 0;
  regex_t regex;
  rcode = regcomp(&regex, META_FILE_NAME_FORMAT, REG_NEWLINE | REG_EXTENDED);
  if(rcode != 0) {
    return NULL;
  }

  DIR *p_dir;
  struct dirent *p_dirent;
  if((p_dir = opendir(root)) == NULL) {
    return NULL;
  }

  int32_t count = 0;
  while((p_dirent = readdir(p_dir)) != NULL) {
    if(*p_dirent->d_name == '.') {
      continue;
    }

    regmatch_t _match;
    if(regexec(&regex, p_dirent->d_name, 1, &_match, 0) == 0) {
      ++count;
    }
  }
  closedir(p_dir);

  if(p_nmetas != NULL) {
    *p_nmetas = count;
  }
  struct realsense_meta_t *metas =
    (struct realsense_meta_t *)calloc(count, sizeof(struct realsense_meta_t));

  if((p_dir = opendir(root)) == NULL) {
    return NULL;
  }

  int32_t cursor = 0;
  regmatch_t matches[5];
  const size_t nmatch = sizeof(matches) / sizeof(matches[0]);
  while((p_dirent = readdir(p_dir)) != NULL) {
    char *_name = p_dirent->d_name;
    if(*_name == '.') {
      continue;
    }

    memset((void *)matches, 0, sizeof(regmatch_t) * nmatch);
    rcode = regexec(&regex, p_dirent->d_name, nmatch, matches, 0);
    if(rcode == 0) {
      (metas + cursor)->serial_number =
        strntol(_name + matches[1].rm_so, NULL, 10, matches[1].rm_eo - matches[1].rm_so);

      (metas + cursor)->frame_number =
        strntol(_name + matches[2].rm_so, NULL, 10, matches[2].rm_eo - matches[2].rm_so);

      (metas + cursor)->stream =
        string_ncopy(_name + matches[3].rm_so, matches[3].rm_eo - matches[3].rm_so);

      (metas + cursor)->format =
        string_ncopy(_name + matches[4].rm_so, matches[4].rm_eo - matches[4].rm_so);

      ++cursor;
    }
  }

  regfree(&regex);

  return metas;
}

void *metadata_free(struct realsense_meta_t *this) {
  free((void *)this->format);
  free((void *)this->stream);

  memset((void *)this, 0, sizeof(struct realsense_meta_t));

  return NULL;
}
