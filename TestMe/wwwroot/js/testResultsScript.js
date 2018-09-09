function rateTest(testId, mark) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    var myData = { mark: mark };
    var testIdData = { id: testId };
    var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(testIdData, dataWithAntiforgeryToken);
    $.ajax({
        url: "/TestEngine/RateFinishedTestAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            if (data) {
                $('#dislikeButton' + testId).removeClass('btn-primary');
                $('#likebutton' + testId).addClass('btn-primary');
            }
            else {
                $('#likebutton' + testId).removeClass('btn-primary');
                $('#dislikeButton' + testId).addClass('btn-primary');
            }
        },
        error: function () {
            
        }
    });
}
