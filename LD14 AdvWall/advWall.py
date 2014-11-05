"""
    Advancing Wall of Doom
"""
from math import pi, sin, cos, sqrt
from pyglet.gl import *
import pyglet, math

class Tower:
    def __init__(self, radius, slices, floorHeight, floors):
        # Raw Data
        self.vertices = []
        self.normals = []
        self.indexs = []
        
        # Gen Verts        
        cir_step = 2 * pi / (slices);
        
        for level in range(floors):
            z = level * floorHeight;
            rad = radius * ((level+1.0) / floors);
            cir_point = 0;
            print radius, rad , level , floors;
            
            for i in range(slices):            
                x = rad * cos(cir_point);
                y = rad * sin(cir_point);
                
                length = sqrt(x*x + y*y);
                nx = -(x / length);
                ny = -(y / length);
                nz = 0;
                
                self.vertices.extend([x, y, z])
                self.normals.extend([nx, ny, nz])
                
                cir_point += cir_step;
                
            # End of Floor
        # Enf of Tower
        
        print self.vertices;
        
        # Gen Indexs
        l = 0;
        while(l < (floors-1)):
            i = 0;
            while(i < slices):
                botLeft = i + l*slices;
                botRight = (i + 1) % slices + l*slices;
                tri = [botLeft, botLeft+slices, botRight];
                print tri;
                self.indexs.extend(tri);
                tri = [botRight, botLeft+slices, botRight+slices];
                print tri;
                self.indexs.extend(tri);
                i += 1;
                
            l += 1;
                
        # Convert to C style Data
        self.vertices = (GLfloat * len(self.vertices))(*self.vertices)
        self.normals = (GLfloat * len(self.normals))(*self.normals)
        self.indexs = (GLuint * len(self.indexs))(*self.indexs)
        
        # Compile a display list
        self.list = glGenLists(1)
        glNewList(self.list, GL_COMPILE)

        glPushClientAttrib(GL_CLIENT_VERTEX_ARRAY_BIT)
        glEnableClientState(GL_VERTEX_ARRAY)
        glEnableClientState(GL_NORMAL_ARRAY)
        glVertexPointer(3, GL_FLOAT, 0, self.vertices)
        glNormalPointer(GL_FLOAT, 0, self.normals)
        glDrawElements(GL_TRIANGLES, len(self.indexs), GL_UNSIGNED_INT, self.indexs)
        glPopClientAttrib()

        glEndList()
        
        return;
        
    def update(self):
        self.radius -= 0.1;
        return;
        
    def draw(self):
        glCallList(self.list);
        return;

        
try:
    # Try and create a window with multisampling (antialiasing)
    config = Config(sample_buffers=1, samples=4, 
                    depth_size=16, double_buffer=True,)
    window = pyglet.window.Window(resizable=True, config=config)
except pyglet.window.NoSuchConfigException:
    # Fall back to no multisampling for old hardware
    window = pyglet.window.Window(resizable=True)

@window.event
def on_resize(width, height):
    # Override the default on_resize handler to create a 3D projection
    glViewport(0, 0, width, height)
    glMatrixMode(GL_PROJECTION)
    glLoadIdentity()
    gluPerspective(60., width / float(height), .1, 1000.)
    glMatrixMode(GL_MODELVIEW)
    return pyglet.event.EVENT_HANDLED

def update(dt):
    global rx, ry, rz, tower;
    
    rx += 1
    ry = 0
    rz = 0
    return;
pyglet.clock.schedule(update)

@window.event
def on_draw():
    global rx, ry, rz, tower;
    
    glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT)
    glLoadIdentity()
    glTranslatef(0, 0, -4)
    glRotatef(rz, 0, 0, 1)
    glRotatef(ry, 0, 1, 0)
    glRotatef(rx, 1, 0, 0)
    tower.draw()
        
def vec(*args):
    return (GLfloat * len(args))(*args)
        
# Setup Pyglet
def setup():
    glClearColor(1, 1, 1, 1)
    glColor3f(1, 0, 0)
    glEnable(GL_DEPTH_TEST)
    glEnable(GL_CULL_FACE)

    # Simple light setup.  On Windows GL_LIGHT0 is enabled by default,
    # but this is not the case on Linux or Mac, so remember to always 
    # include it.
    glEnable(GL_LIGHTING)
    glEnable(GL_LIGHT0)
    glEnable(GL_LIGHT1)

    # Define a simple function to create ctypes arrays of floats:


    glLightfv(GL_LIGHT0, GL_POSITION, vec(0.5, 0.5, 1.0, 0.0))
    glLightfv(GL_LIGHT0, GL_SPECULAR, vec(0.5, 0.5, 1.0, 1.0))
    glLightfv(GL_LIGHT0, GL_DIFFUSE,  vec(1.0, 1.0, 1.0, 1.0))
    glLightfv(GL_LIGHT1, GL_POSITION, vec(1.0, 0.0, 0.5, 0.0))
    glLightfv(GL_LIGHT1, GL_DIFFUSE,  vec(0.5, 0.5, 0.5, 1.0))
    glLightfv(GL_LIGHT1, GL_SPECULAR, vec(1.0, 1.0, 1.0, 1.0))

    glMaterialfv(GL_FRONT_AND_BACK, GL_AMBIENT_AND_DIFFUSE, vec(0.5, 0.0, 0.3, 1.0))
    glMaterialfv(GL_FRONT_AND_BACK, GL_SPECULAR, vec(1, 1, 1, 1))
    glMaterialf(GL_FRONT_AND_BACK, GL_SHININESS, 50)

setup()
tower = Tower(20, 18, 4, 8);
rx = 0
ry = 0
rz = 0

pyglet.app.run()