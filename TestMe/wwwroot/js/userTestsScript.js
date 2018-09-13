function getTopTests() {
    var token = $('input[name="__RequestVerificationToken"]', $('#userTests')).val();
    var skipAmount = { skipAmount: $('#userTests tr').length - 1 };
    var amount = { amount: 10 };
    var searchString = { searchString: getUrlParameter("SearchString") };
    var userId = { userId: $('input[name="userId"]').val() };
    var dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(userId, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 

    $.ajax({
        url: "/Profile/GetUserProfileTestsAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendTopTests(data);
        },
        error: function () {

        }
    });
}
function appendTopTests(tests) {
    var table = $('#userTests');
    tests.forEach(function (element) {
        var tr = $('<tr/>');
        var testRef = $('<a/>', { href: '/' + element.testCode, text: element.testName });
        var testTd = $('<td/>').append(testRef);
        tr.append(testTd);
        var formattedDate = new Date(element.creationDate);
        tr.append($('<td/>', { text: formattedDate.toLocaleString() }));
        tr.append($('<td/>', { text: element.testDuration.slice(0, -3) }));
        var rateClass = '';
        if (element.testRating > 0)
            rateClass = 'text-success';
        else if (element.testRating < 0)
            rateClass = 'text-danger';
        var td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        appendTopTestsControls(tr, element);
        table.append(tr);
    });
}
function appendTopTestsControls(tr, test) {
    var detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    var td = $('<td/>').append(detailsRef);
    tr.append(td);
}