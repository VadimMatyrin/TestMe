// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function resetPhotoButtonClick() {
    var oldSrc = $('#answerImage').data('src');
    $('#answerImage').attr('src', oldSrc);
    if (oldSrc.length === 0)
        $('#answerImage').hide();
    $('#resetPhotoButton').hide();
    $('#ImageName').val("");
}
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#answerImage').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

$("#ImageName").change(function () {
    readURL(this);
    $('#answerImage').show();
    $('#resetPhotoButton').show();
});