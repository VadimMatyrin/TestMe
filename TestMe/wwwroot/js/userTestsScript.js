const amount = { amount: 10 };
function getUserSharedTests() {
    let token = $('input[name="__RequestVerificationToken"]', $('#userTests')).val();
    let skipAmount = { skipAmount: $('#userTests tr').length - 1 };
    let searchString = { searchString: getUrlParameter("searchString") };
    let userId = { userId: $('input[name="userId"]').val() };
    let dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(userId, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 

    $.ajax({
        url: "/Profile/GetUserProfileTestsAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendUserTests(data);
        },
        error: function () {

        }
    });
}
function appendUserTests(tests) {
    if (tests.length === 0 || tests.length !== amount.amount) {
        let button = $('#loadMoreButton');
        button.unbind("click");
        button.prop({ disabled: true });
        return;
    }
    let table = $('#userTests');
    tests.forEach(function (element) {
        let tr = $('<tr/>');
        let testRef = $('<a/>', { href: '/' + element.testCode, text: element.testName });
        let testTd = $('<td/>').append(testRef);
        tr.append(testTd);
        let formattedDate = new Date(element.creationDate);
        let timeTd = $('<td/>', { text: formattedDate.toLocaleString(), class: 'hidden-xs' });
        tr.append(timeTd);
        tr.append($('<td/>', { text: element.testDuration.slice(0, -3) }));
        let rateClass = '';
        if (element.testRating > 0)
            rateClass = 'text-success';
        else if (element.testRating < 0)
            rateClass = 'text-danger';
        var td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        appendUserTestsControls(tr, element);
        table.append(tr);
    });
}
function appendUserTestsControls(tr, test) {
    let detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    let td = $('<td/>').append(detailsRef);
    tr.append(td);
}
$('#loadMoreButton').click(function (e) {
    getUserSharedTests();
});