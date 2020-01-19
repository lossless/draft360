var scenes = [
  [
    "Grid",
    "https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2F3c9ad69e-2641-4071-989a-13823b3d8c62.image.png?v=1579384471820"
  ],
  [
    "City",
    "https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2Fcityscape1080px.png?v=1579449483178"
  ],
  [
    "Team 009",
    "https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2F8b4718d5-af4e-4720-b09b-9c4f4a59768f.image.png?v=1579359942179"
  ]
];
var images = [
  [
    "Person",
    "https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2Fstamp_person1.png?v=1579396695751"  
  ],
  [
    // temp image for multiple-images testing
    "Pigeon",
    "https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2Fpigeon-transparent.png?v=1579407367303"
  ],
  [
    "Portal",
    "https://cdn.glitch.com/dff38557-346e-4aa3-94d5-969225a03cf0%2FPortal.png?v=1579443744370"
  ]
];

function pickFile() {
  var input = document.createElement("input");
  input.setAttribute("type", "file");
  return new Promise((ok, err) => {
    input.onchange = () => { 
      var file = input.files[0];
      var reader = new FileReader();
      reader.onload = () => {
        var content = reader.result;
        ok([file.name, content]);
      }
      reader.readAsDataURL(file);
    }
    input.click();
  });
}

window.addEventListener("load", () => {
  document.getElementById("360-open").addEventListener("click", () => {
    pickFile().then(([name, content]) => {
      scenes.push([name, content]);
    });
  });
  document.getElementById("image-open").addEventListener("click", () => {
    pickFile().then(([name, content]) => {
      images.push([name, content]);
    });
  });
});
