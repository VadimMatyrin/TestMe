const amount = { amount: 10 };
function getTopTests() {
    let token = $('input[name="__RequestVerificationToken"]', $('#topTestTable')).val();
    let skipAmount = { skipAmount: $('#topTestTable tr').length - 1 };
    let searchString = { searchString: getUrlParameter("searchString") };
    let dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 
    $.ajax({
        url: "/Tests/GetTopTestsAjax",
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
    if (tests.length === 0 || tests.length !== amount.amount) {
        let button = $('#loadMoreButton');
        button.unbind("click");
        button.prop({ disabled: true });
        return;
    }
    let table = $('#topTestTable');
    tests.forEach(function (element) {
        let tr = $('<tr/>');
        let testRef = $('<a/>', { href: '/TestEngine/' + element.testCode, text: element.testName });
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
        let td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        appendTopTestsControls(tr, element);
        table.append(tr);
    });
}
function appendTopTestsControls(tr, test) {
    let detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    let td = $('<td/>').append(detailsRef);
    tr.append(td);
}

$('#loadMoreButton').click(function (e) {
    getTopTests();
});