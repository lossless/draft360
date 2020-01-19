/* global AFRAME, THREE */
AFRAME.registerComponent('renderer', {
  schema: {
    radius: {default: 100},
    subdiv: {default: 64},
    strokeColor: {default: "red"},
    strokeSize: {default: 3},
    hThreshold: {default: 0.8},
    enabled: {default: true}
  },
  deleteFrame: function(scene) {
    this.canvases.splice(scene, 1);
    this.images.splice(scene, 1);
    this.meshes.splice(scene, 1);
    this.textures.splice(scene, 1);
    this.ctxs.splice(scene, 1);
  },
  loadImage: function(img, scene) {
    if(this.images[scene]) {
    this.image = this.images[scene];                    
      this.canvas = this.canvases[scene];
      if(this.canvases[scene]) {
        this.loaded = true;
        this.el.setObject3D("mesh", this.meshes[scene]);
        this.ctx = this.ctxs[scene];
        this.texture = this.textures[scene];
      }
      return;
    }
    this.image = new Image();
    this.image.crossOrigin = "anonymous";
    this.image.src = img;
    this.loaded = false;
    this.images[scene] = this.image;
    this.image.onload = (() => {
      var canvas = document.createElement("canvas");
      canvas.width = this.image.width;
      canvas.height = this.image.height;
      var ctx = canvas.getContext("2d");
      ctx.clearRect(0, 0, canvas.width, canvas.height);
      ctx.strokeStyle = this.data.strokeColor;
      ctx.lineWidth = this.data.strokeSize;
      this.ctx = ctx;
      this.ctx.drawImage(this.image, 0, 0);
      this.canvases[scene] = canvas;
      this.ctxs[scene] = ctx;
      this.canvas = canvas;
      this.texture = new THREE.Texture(this.canvas);
      this.textures[scene] = this.texture;
      var material = new THREE.MeshBasicMaterial({
        map: this.texture,
        flatShading: THREE.FlatShading,
        side: THREE.BackSide
      });
      var geometry = new THREE.SphereGeometry(this.data.radius,
                                              this.data.subdiv,
                                              this.data.subdiv);
      var mesh = new THREE.Mesh(geometry, material);
      this.meshes[scene] = mesh;
      this.el.setObject3D("mesh", mesh);
      this.loaded = true;
      this.texture.needsUpdate = true;
    }).bind(this);
  },
  init: function() {
    this.loaded = false;
    this.canvases = [];
    this.images = [];
    this.meshes = [];
    this.textures = [];
    this.ctxs = [];
    this.line = [];
    this.lines = [];
    this.camera = document.getElementById("camera");
    this.prevEnabled = false;
  },
  tick: function(t) {
    document.getElementById("cover").style.display = this.loaded ? "none" : "block";
    if(!this.loaded) return;
    if(!this.data.enabled) {
      this.prevEnabled = false;
      return;
    }
    if(!this.prevEnabled) {
      this.line = [];
      this.lines.push(this.line);
    }
    this.prevEnabled = true;
    var rot = this.camera.getAttribute("rotation");
    var x = (rot.y + 90) / 360 + 0.5;
    var y = rot.x / -180 + 0.5;
    x -= Math.floor(x);
    x *= this.canvas.width;
    y *= this.canvas.height;
    x = Math.floor(x);
    y = Math.floor(y);
    this.line.push([x, y]);
    if(this.line.length > 1) {
      var i = this.line.length - 1;
      if(Math.abs(this.line[i][0] - this.line[i - 1][0])
         > this.canvas.width * this.data.hThreshold) return;
      this.ctx.beginPath();
      this.ctx.moveTo(this.line[i][0], this.line[i][1]);
      this.ctx.lineTo(this.line[i - 1][0], this.line[i - 1][1]);
      this.ctx.stroke();
      this.texture.needsUpdate = true;
    }
  }
});