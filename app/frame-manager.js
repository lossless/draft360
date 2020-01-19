/* global AFRAME THREE closeDialog showDialog showQRDialog scenes images */
var prefix = "https://team-009.glitch.me"

AFRAME.registerComponent('frame-manager', {
  schema: {
    portalDistance: {default: 3},
    portalRadius: {default: 0.1}
  },
  init: function() {
    this.loaded = true;
    var href = window.location.href;
    if(href.includes("draft")) {
      this.loaded = false;
      var parts = href.split('/');
      var lastSegment = parts.pop() || parts.pop();
      var url = prefix + "/file/" + lastSegment;
      fetch(url).then(res => res.json()).then(res => {
        this.frames = res;
        this.loaded = true;
      })
    .then(response => {
        // handle response data
    })
    }
    this.frames = [
      {
        //needs to change with upload functionality // of course
        portals: [],
        images: [],
        texts: [],
        base: scenes[0][1]
      }
    ];
    this.frame = 0;
        document.getElementById("frames").onclick = () => {
      var buttons = this.frames.map((frame, index) => [
        index + 1, () => {
          this.frame = index;
          closeDialog();
        }
      ]);
      showDialog("Frame:", buttons);
    };
    document.getElementById("export").onclick = () => {
      //console.log("export button clicked");
      var json = this.frames.map(({portals, images, texts}, index) => ({
        portals, images, texts, base: document.getElementById("renderer").components.renderer.canvases[index].toDataURL("image/png")
      }));
      var xhr = new XMLHttpRequest();
      xhr.open("POST", prefix + "/store/", true);
      xhr.setRequestHeader('Content-Type', 'application/json');
      console.log(JSON.stringify(json));
      xhr.send(JSON.stringify(json));
      xhr.onreadystatechange = function() {
        if (xhr.readyState == XMLHttpRequest.DONE) {
          showQRDialog(prefix + "/draft/" + xhr.responseText);
        }
      }
    }
    document.getElementById("new-frame").onclick = () => {
      showDialog("Choose 360 backdrop for your scene:", scenes.map(([name, url]) => [
        name, () => {
          closeDialog();
          this.frame = this.frames.length;
          this.frames.push({
            base: url,
            portals: [],
            images: [],
            texts: []
          });
        }
      ]));
    };
    document.getElementById("delete-frame").onclick = () => {
      showDialog("Are you sure you want to delete this frame? There's no way to undo this action", [
        ["Yes", () => {
          closeDialog();
          if(this.frames.length < 2) return;
          this.frames.splice(this.frame, 1);
          document.getElementById("renderer").components.renderer.deleteFrame(this.frame);
          this.frame = 0;
        }]
      ])
    };
    document.getElementById("new-portal").onclick = () => {
      var buttons = this.frames.map((frame, index) => [
        index + 1, () => {
          closeDialog();
          showDialog("Choose image for your Portal:", images.map(([name, image]) => [
            name, () => {
              var cursor = document.getElementById("cursor");
              var worldPos = new THREE.Vector3();
              worldPos.setFromMatrixPosition(cursor.object3D.matrixWorld);
              worldPos.multiplyScalar(this.data.portalDistance);
              var rot = document.getElementById("camera").object3D.rotation;
              this.frames[this.frame].portals.push({
                position: worldPos,
                rotation: rot,
                src: image,
                to: index
              });
              closeDialog();
            }
          ]));
          /*
          var cursor = document.getElementById("cursor");
          var worldPos = new THREE.Vector3();
          worldPos.setFromMatrixPosition(cursor.object3D.matrixWorld);
          worldPos.multiplyScalar(this.data.portalDistance);
          this.frames[this.frame].portals.push({
            position: worldPos,
            to: index
          });
          closeDialog();
          */
        }
      ]);
      showDialog("Link portal to:", buttons);
    };
    this.imageMode = false;
    document.getElementById("image-mode").onclick = () => {
      /*if (this.imageMode) {
        this.imageMode = false;
        document.getElementById("image-mode-icon").innerText = "add_photo_alternate";
      } else {
      */
        showDialog("Choose image:", images.map(([name, image]) => [
          name, () => {
            this.imageMode = true;
            //document.getElementById("image-mode-icon").innerText = "done";
            var cursor = document.getElementById("cursor");
            var worldPos = new THREE.Vector3();
            worldPos.setFromMatrixPosition(cursor.object3D.matrixWorld);
            worldPos.multiplyScalar(this.data.portalDistance);
            var rot = document.getElementById("camera").object3D.rotation;
            this.frames[this.frame].images.push({
              position: worldPos,
              rotation: rot,
              src: image
            });
            closeDialog();
          }
        ]));
      //}
    }
    document.getElementById("text-mode").onclick = () => {
      document.getElementById("camera").setAttribute("mylookcontrols", "useSpace", false);
      showDialog(`Write some text here:
                  <br>
                  <div class="mdl-textfield mdl-js-textfield">
                    <input class="mdl-textfield__input" type="text" id="text-field">
                  </div>`, [
        ["Ok", () => {
          document.getElementById("camera").setAttribute("mylookcontrols", "useSpace", true);
          var cursor = document.getElementById("cursor");
          var worldPos = new THREE.Vector3();
          worldPos.setFromMatrixPosition(cursor.object3D.matrixWorld);
          worldPos.multiplyScalar(this.data.portalDistance);
          var rot = document.getElementById("camera").object3D.rotation;
          console.log(rot);
          this.frames[this.frame].texts.push({
            position: worldPos,
            rotation: rot,
            text: document.getElementById("text-field").value
          });
          closeDialog();
        }
      ]]);
      //}
    }
  },
  tick: function() {
    if(!this.loaded) return;
    document.getElementById("frame-number").innerText = this.frame + 1
    document.getElementById("renderer").components.renderer
      .loadImage(this.frames[this.frame].base, this.frame);
    for(var i = 0; i < this.frames.length; i++) {
      var { portals, images, texts } = this.frames[i];
      portals.forEach(({ position, rotation, src, to }, nd) => {
        var portalId = `portal-${i}-${nd}`;
        var elem = document.getElementById(portalId);
        if(i == this.frame) {
          if(!elem) {
            /*
            var portal = document.createElement("a-sphere");
            var p = position;
            portal.setAttribute("position", `${p.x} ${p.y} ${p.z}`);
            portal.setAttribute("radius", this.data.portalRadius);
            portal.id = portalId;
            portal.onclick = () => {
              this.frame = to;
            };
            this.el.sceneEl.appendChild(portal);
            */
            var portal = document.createElement("a-image");
            portal.id = portalId;
            //portal.setAttribute("size", `200 200`);
            portal.setAttribute("src", src);
            // portal.setAttribute("particle-system", "color: #44CC00; maxAge: 0.1; particleCount: 10; velocityValue: 0 0 0; velocitySpread: 1 1 1");
            //portal.setAttribute("animation", "property: scale; dur: 1000; from: 1 1 1; to: 0.8 0.8 0.8; loop: true; direction: alternate; easing: linear");
            portal.onclick = () => {
              this.frame = to;
            };
            this.el.sceneEl.appendChild(portal);
            var text = document.createElement("a-entity");
            text.setAttribute("text", `width: 4; color: white; align: center; value: Portal to ${to + 1}`);
            portal.appendChild(text);
            var p = position;
            portal.setAttribute("position", `${p.x} ${p.y} ${p.z}`);
            var r = rotation;
            portal.setAttribute("rotation", `${r.x} ${r.y} ${r.z}`);
          } else {
            elem.setAttribute("look-at", "[camera]");
          }
        } else if(!!elem) elem.remove();
      });
      images.forEach(({ position, rotation, src }, nd) => {
        var stampId = `image-${i}-${nd}`;
        var elem = document.getElementById(stampId);
        if(i == this.frame) {
          if(!elem) {
            var stampImg = document.createElement("a-image");
            stampImg.id = stampId;
            //stampImg.setAttribute("size", `200 200`);
            stampImg.setAttribute("src", src);
            this.el.sceneEl.appendChild(stampImg);
            var p = position;
            stampImg.setAttribute("position", `${p.x} ${p.y} ${p.z}`);
            var r = rotation;
            stampImg.setAttribute("rotation", `${r.x} ${r.y} ${r.z}`);
          } else {
            elem.setAttribute("look-at", "[camera]");
          }
        } else if(!!elem) elem.remove();
      });
      texts.forEach(({ position, rotation, text }, nd) => {
        var textId = `text-${i}-${nd}`;
        var elem = document.getElementById(textId);
        if(i == this.frame) {
          if(!elem) {
            var txt = document.createElement("a-entity");
            txt.id = textId;
            txt.setAttribute("text", `width: 2; wrapCount: 10; color: white; align: center; value: ${text}`);
            this.el.sceneEl.appendChild(txt);
            var p = position;
            txt.setAttribute("position", `${p.x} ${p.y} ${p.z}`);
            var r = rotation;
            txt.setAttribute("rotation", `${r.x} ${r.y} ${r.z}`);
          } else {
            elem.setAttribute("look-at", "[camera]");
          }
        } else if(!!elem) elem.remove();
      });
    }
  }
});