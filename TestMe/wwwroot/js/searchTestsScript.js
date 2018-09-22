const amount = { amount: 10 };
function getTests() {
    let token = $('input[name="__RequestVerificationToken"]', $('#testTable')).val();
    let skipAmount = { skipAmount: $('#testTable tr').length - 1 };
    let searchString = { searchString: getUrlParameter("searchString") };

    let testRatingFrom = { testRatingFrom: getUrlParameter("testRatingFrom") };
    let testRatingTo = { testRatingTo: getUrlParameter("testRatingTo") };

    let testDurationFrom = { testDurationFrom: getUrlParameter("testDurationFrom") };
    let testDurationTo = { testDurationTo: getUrlParameter("testDurationTo") };

    let dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });

    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(searchString, dataWithAntiforgeryToken); 

    dataWithAntiforgeryToken = $.extend(testRatingFrom, dataWithAntiforgeryToken); 
    dataWithAntiforgeryToken = $.extend(testRatingTo, dataWithAntiforgeryToken); 

    dataWithAntiforgeryToken = $.extend(testDurationFrom, dataWithAntiforgeryToken);
    dataWithAntiforgeryToken = $.extend(testDurationTo, dataWithAntiforgeryToken); 
    $.ajax({
        url: "/Tests/GetSharedTestsAjax",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendTests(data);
        },
        error: function () {

        }
    });
}
function appendTests(tests) {
    let table = $('#testTable');
    if (tests.length === 0 || tests.length !== amount.amount) {
        let button = $('#loadMoreButton');
        button.unbind("click");
        button.prop({ disabled: true });
        return;
    }

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
        let td = $('<td/>').append($('<span/>', { text: element.testRating, class: rateClass }));
        tr.append(td);
        appendTestsControls(tr, element);
        table.append(tr);
    });
}
function appendTestsControls(tr, test) {
    let detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    let td = $('<td/>').append(detailsRef);
    tr.append(td);
}
$('#resetFilterButton').click(function (e) {
    ResetFilters(e);
});
$('#loadMoreButton').click(function (e) {
    getTests();
});

function ResetFilters(e) {
    e.stopPropagation();
    $('input[name="testRatingFrom"]').val('');
    $('input[name="testRatingTo"]').val('');
    $('input[name="testDurationFrom"]').val('');
    $('input[name="testDurationTo"]').val('');
}