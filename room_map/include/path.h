#ifndef _PATH_H
#define _PATH_H

/**
 */
extern char *path_join(
    char *p1,
    char *p2);

extern char *paths_join_allocate(size_t argc, char separator, char *p1, ...);

#endif
