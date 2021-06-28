CC = gcc
FLAGS =
PRJC = room_map
OBJDIR = objs

SRC = src/main.c \
      src/io.c \
      src/path.c \
      src/json.c \
      src/exlib.c \
      src/exstring.c \
      src/device_info.c \
      src/realsense_meta.c \
			src/ply.c \
      ../parson/parson.c

LINK = -lm \
       -lpng
DEFINE = -D__DEBUG__


OBJS = $(addprefix $(OBJDIR)/,$(patsubst %.c,%.o,$(SRC)))
OBJDIRS = $(sort $(dir $(OBJS)))

.PHONY: default $(PRJC) clean all run

default:
	[ -d $(OBJDIR) ] || mkdir -v $(OBJDIR)
	[ -d "$(OBJDIRS)" ] || mkdir -pv $(OBJDIRS)
	make $(PRJC)

$(PRJC):$(OBJS)
	$(CC) -o $@ $^ $(FLAGS) \
	  $(LINK)

$(OBJDIR)/%.o:%.c
	$(CC) -o $@ -c $< $(FLAGS) \
	  $(DEFINE)

all:
	make clean default

clean:
	[ ! -f $(PRJC) ] || rm -v $(PRJC)
	[ ! -d $(OBJDIR) ] || rm -rv $(OBJDIR)

run:
	./$(PRJC) \
		-n 16 \
		-t ../RoomMap/RoomMap.Cmd/tmp/acfed9aa-1359-4bc2-bbb2-97c1fe2d6e0a
