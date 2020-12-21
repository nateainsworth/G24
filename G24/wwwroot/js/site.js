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


    

    var error_elm = document.getElementById('login-error');
    if (error_elm) {
        var isEmpty = error_elm.innerHTML === "";
        if (isEmpty) {
            document.getElementById('login-error').classList.add("hide");
        }
    }


    element = document.getElementById('site-loader')
    element.classList.add("fade");

};

function DeleteWarningPanel() {
    var Modal = document.getElementById("DeleteModal");
    Modal.classList.toggle("show");
    var Modal_Backdrop = document.getElementById("modal-backdrop");
    Modal_Backdrop.classList.toggle("show");
    Modal_Backdrop.classList.toggle("hide");


    var checkedBoxes = document.querySelectorAll('input[type="checkbox"]:checked');
    if (checkedBoxes.length > 0) {

        var tr = document.createElement('tr');
        tr.setAttribute("class", "delete_table_row");

        for (var i = 0; i < checkedBoxes.length; i++) {
            //checkedBoxes[i].checked = true;   
            var checkboxID = checkedBoxes[i].id;
            var IDSplit = checkboxID.split('_');
            var lineID = IDSplit[1];

            var td = document.createElement('td');
            var div = document.createElement('div');
            div.setAttribute("class", "delete_table_cont");
            var p = document.createElement('p');
            p.appendChild(document.createTextNode('Id of: ' + document.getElementById("Img_" + lineID + "__ImgID").value));
            p.setAttribute("class", "delete_table_text");
            var img = document.createElement("IMG");
            img.setAttribute("class", "delete_table_img");
            img.setAttribute("src", document.getElementById("img_ID_" + lineID).getAttribute('src'));
            div.appendChild(img);
            div.appendChild(p);
            td.appendChild(div);//document.createTextNode('Id of:')

            tr.appendChild(td);

        }
        // var deleteList = document.getElementById("DdeleteList");
        document.getElementById('deleteList').innerHTML = "";
        document.getElementById('deleteList').append(tr);
        document.getElementById("DeleteMessage").innerHTML = "Are you sure you want to delete the list of images below?";
        document.getElementById("DeleteImgSubmit").disabled = false;
    } else {
        document.getElementById("DeleteMessage").innerHTML = "No images selected";
        document.getElementById("DeleteImgSubmit").disabled = true;
    }
}