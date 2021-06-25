#include <stdio.h>
#include <string.h>
#include <stdlib.h>
#include <unistd.h>
#include <stdint.h>
#include "../parson/parson.h"
#include "../include/device_info.h"


static char *_strcpy_new(const char *str);

struct device_info_t *device_info_get(const char *path) {
  struct device_info_t *p_dev = NULL;
  JSON_Value *p_json = json_parse_file(path);
  JSON_Object *p_obj = json_value_get_object(p_json);


  p_dev = (struct device_info_t *)calloc(1, sizeof(struct device_info_t));
  p_dev->name = _strcpy_new(json_object_get_string(p_obj, "name"));
  p_dev->product_id = _strcpy_new(json_object_get_string(p_obj, "productId"));
  p_dev->firmware_version = _strcpy_new(json_object_get_string(p_obj, "firmwareVersion"));
  p_dev->depth_scale = json_object_get_number(p_obj, "depthScale");

  JSON_Array *p_profiles = json_object_get_array(p_obj, "profiles");
  if(p_profiles != NULL) {
    p_dev->profile_count = json_array_get_count(p_profiles);

    p_dev->profiles = (struct profile_t *)calloc(p_dev->profile_count, sizeof(struct profile_t));
    for(int i = 0; i < p_dev->profile_count; ++i) {
      JSON_Object *p_profile = json_array_get_object(p_profiles, i);
      struct profile_t *_profile = (p_dev->profiles + i);

      _profile->width = (int32_t)json_object_get_number(p_profile, "width");
      _profile->height = (int32_t)json_object_get_number(p_profile, "height");
      _profile->framerate = (int16_t)json_object_get_number(p_profile, "framerate");
      _profile->stream = _strcpy_new(json_object_get_string(p_profile, "stream"));
      _profile->format = _strcpy_new(json_object_get_string(p_profile, "format"));

    }
  }
  json_value_free(p_json);

  return p_dev;
}

void device_info_free(struct device_info_t *this) {
  free((void *)this->name);
  free((void *)this->firmware_version);
  free((void *)this->product_id);

  free((void *)this->profiles);
  free((void *)this);
}



static char *_strcpy_new(const char *str) {
  if(str == NULL) {
    return NULL;
  }

  size_t size = strlen(str);

  char *p = (char *)calloc(size + 1, sizeof(char));
  strncpy(p, str, size);

  return p;
}
