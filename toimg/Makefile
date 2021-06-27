CC = gcc
FLAGS =
OBJDIR = objs
PRJC = toimg

LINKS = -ljpeg
SRC = src/main.c \
			src/jpeg.c

OBJS = $(addprefix $(OBJDIR)/,$(patsubst %.c,%.o,$(SRC)))
OBJDIRS = $(sort $(dir $(OBJS)))


.PHONEY: default clean run

default:
	[ -d $(OBJDIR) ] || mkdir -v $(OBJDIR)
	[ -d "$(OBJDIRS)" ] || mkdir -pv $(OBJDIRS)
	make $(PRJC)

$(PRJC):$(OBJS)
	$(CC) -o $@ $^ \
		$(LINKS) \
		$(FLAGS)

$(OBJDIR)/%.o:%.c
	$(CC) -o $@ -c $< \
		$(FLAGS)

run:
	./$(PRJC) \
		-s 1280x720 \
		-i ../RoomMap/RoomMap.Cmd/tmp/33e51c70-8fb7-42ba-b537-d965b00dd5c2/24.24.Color-Rgb8
