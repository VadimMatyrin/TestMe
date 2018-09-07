function getTopTests() {
    var token = $('input[name="__RequestVerificationToken"]', $('#topTestTable')).val();
    var skipAmount = { skipAmount: $('#topTestTable tr').length };
    var amount = { amount: 10 };
    var dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken);
    $.ajax({
        url: "/Tests/GetTopTests",
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
    var table = $('#topTestTable');
    tests.forEach(function (element) {
        var tr = $('<tr/>');
        var testRef = $('<a/>', { href: '/TestEngine/' + element.testCode, text: element.testName });
        var testTd = $('<td/>').append(testRef);
        tr.append(testTd);
        var formattedDate = new Date(element.creationDate);
        tr.append($('<td/>', { text: formattedDate.toLocaleString() }));
        tr.append($('<td/>', { text: element.duration.slice(0, -3) }));
        tr.append($('<td/>', { text: element.testRating }));
        appendTopTestsControls(tr, element);
        table.append(tr);
    });
}
function appendTopTestsControls(tr, test) {
    var detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    var td = $('<td/>').append(detailsRef);
    tr.append(td);
}