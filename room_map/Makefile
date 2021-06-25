CC = gcc
FLAGS =
PRJC = room_map
OBJDIR = objs

SRC = src/main.c \
      src/io.c \
      src/path.c \
			src/json.c \
			parson/parson.c \
			src/device_info.c

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
	./$(PRJC) -t ../RoomMap/RoomMap.Cmd/tmp/fe5de5ec-f40c-4fea-a27d-c82774feeda7
