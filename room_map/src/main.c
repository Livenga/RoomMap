#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <getopt.h>
#include <unistd.h>

#include "../include/io.h"
#include "../include/path.h"
#include "../parson/parson.h"


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
  JSON_Value* json_root = json_parse_file(p_device_info_path);
  free((void *)p_device_info_path);


  json_value_free(json_root);

  return 0;
}
