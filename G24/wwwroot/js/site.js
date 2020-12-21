
// gets the image which has been uploaded on upload and displays it in the preview box
function readURL(input) {
    var reader = new FileReader();
    reader.onload = function (e) {
        document.getElementById("Img").setAttribute("src", e.target.result);
    };
    reader.readAsDataURL(input.files[0]);

}


window.onload = function () {
    // on load of login page check if error exists if so hide error message
    var error_elm = document.getElementById('login-error');
    if (error_elm) {
        var isEmpty = error_elm.innerHTML === "";
        if (isEmpty) {
            document.getElementById('login-error').classList.add("hide");
        }
    }
    // once page is loaded fade the loading screen
    element = document.getElementById('site-loader')
    element.classList.add("fade");

};

// Delete modal called and edited within javascript, rather than jquery which is normally used within the bootstrap library and would take less lines of code
function DeleteWarningPanel() {
    var Modal = document.getElementById("DeleteModal");
    Modal.classList.toggle("show");
    var Modal_Backdrop = document.getElementById("modal-backdrop");
    Modal_Backdrop.classList.toggle("show");
    Modal_Backdrop.classList.toggle("hide");


    var checkedBoxes = document.querySelectorAll('input[type="checkbox"]:checked');
    if (checkedBoxes.length > 0) {
        //find which check boxes are checked
        var tr = document.createElement('tr');
        tr.setAttribute("class", "delete_table_row");

        for (var i = 0; i < checkedBoxes.length; i++) {
           // if check boxes exists loop through each image to create a column within the table to display each image which is due to be deleted
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
            // append the creation of each element onto  the table row element
            div.appendChild(img);
            div.appendChild(p);
            td.appendChild(div);

            tr.appendChild(td);

        }
        // append table row the new image selection ready for delete
        document.getElementById('deleteList').innerHTML = "";
        document.getElementById('deleteList').append(tr);
        // if images exist display message asking if there sure they want to delete and allow the submit button to be pressed
        document.getElementById("DeleteMessage").innerHTML = "Are you sure you want to delete the list of images below?";
        document.getElementById("DeleteImgSubmit").disabled = false;
    } else {
        // if no images are selected then dsplay message of no images selected and disable use of submit button
        document.getElementById("DeleteMessage").innerHTML = "No images selected";
        document.getElementById("DeleteImgSubmit").disabled = true;
    }
}