#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <getopt.h>
#include <unistd.h>

#include "../include/io.h"
#include "../include/path.h"
#include "../parson/parson.h"
#include "../include/device_info.h"


int main(
    int argc,
    char *argv[]) {
  const struct option opts[] = {
    {"target-directory", required_argument, NULL, 't'},
  };

  if(argc == 0) {
    return -1;
  }


  int opt, opt_index;
  char target_directory[1024];
  char *p = NULL;

  memset((void *)target_directory, 0, sizeof(target_directory));
  while((opt = getopt_long(argc, argv, "t:", opts, &opt_index)) > 0) {
    switch(opt) {
      case 't':
        p = (optarg != NULL) ? optarg : *(argv + optind);
        strncpy(target_directory, p, 1024);
        break;
    }
  }

  fprintf(stderr, "d Target directory: %s\n", target_directory);
  char *p_device_info_path = paths_join_allocate(2, '/', target_directory, "device_info.json");
  struct device_info_t *p_dev = device_info_get(p_device_info_path);

  device_info_free(p_dev);
  memset((void *)p_device_info_path, 0, strlen(p_device_info_path));
  free((void *)p_device_info_path);

  return 0;
}
