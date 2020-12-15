// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
function readURL(input) {
    var reader = new FileReader();

    reader.onload = function (e) {
        document.getElementById("Img").setAttribute("src", e.target.result);
    };
    reader.readAsDataURL(input.files[0]);

}

window.onload = function () {

    element = document.getElementById('site-loader')
    element.classList.add("fade");

};

function DeleteWarningPanel() {
    var Modal = document.getElementById("DeleteModal");
    Modal.classList.toggle("show");
    var Modal_Backdrop = document.getElementById("modal-backdrop");
    Modal_Backdrop.classList.toggle("show");
    Modal_Backdrop.classList.toggle("hide");
}