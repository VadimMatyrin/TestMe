function getTests() {
    var token = $('input[name="__RequestVerificationToken"]', $('#testTable')).val();
    var skipAmount = { skipAmount: $('#testTable tr').length };
    var amount = { amount: 10 };
    var dataWithAntiforgeryToken = $.extend(skipAmount, { '__RequestVerificationToken': token });
    dataWithAntiforgeryToken = $.extend(amount, dataWithAntiforgeryToken); 
    $.ajax({
        url: "/Tests/GetTests",
        type: "POST",
        data: dataWithAntiforgeryToken,
        success: function (data) {
            appendTests(data);
        },
        error: function () {
            //$("#questionForm").empty();
        }
    });
}
function appendTests(tests) {
    var table = $('#testTable');
    tests.forEach(function (element) {
        var tr = $('<tr/>');
        tr.append($('<td/>', { text: element.id }));
        var testNameRef = $('<a/>', { href: '/TestQuestions/Index/' + element.id, text: element.testName});
        var testNameTd = $('<td/>').append(testNameRef);
        tr.append(testNameTd);
        tr.append($('<td/>', { text: element.creationDate }));
        tr.append($('<td/>', { text: element.testCode }));
        tr.append($('<td/>', { text: element.duration.slice(0, -3) }));
        tr.append($('<td/>', { text: element.testRating }));
        tr.append($('<td/>', { text: element.userName }));
        appendControls(tr, element);
        table.append(tr);
    });
}
function appendControls(tr, test) {
    var td = $('<td/>');
    if (test.testCode === null) {
        var editRef = $('<a/>', { href: '/Tests/Edit/' + test.id, text: 'Edit' });
        td.append(editRef);
        td.append('<span> | </span>');
        var deleteRef = $('<a/>', { href: '/Tests/Delete/' + test.id, text: 'Delete' });
        td.append(deleteRef);
        td.append('<span> | </span>');
        var shareTestRef = $('<a/>', { href: '/Tests/CreateCode/' + test.id, text: 'Share test' });
        td.append(shareTestRef);
        td.append('<span> | </span>');
        var validateTestRef = $('<a/>', { href: '/Tests/ValidateTest/' + test.id, text: 'Validate test' });
        td.append(validateTestRef);
        td.append('<span> | </span>');
    }
    else {
        var tryTestRef = $('<a/>', { href: '/TestEngine?code=' + test.testCode, text: 'Try test out' });
        td.append(tryTestRef);
        td.append('<span> | </span>');
        var stopShareRef = $('<a/>', { href: '/Tests/StopSharing/' + test.id, text: 'Stop sharing' });
        td.append(stopShareRef);
        td.append('<span> | </span>');
    }
    var userResultsRef = $('<a/>', { href: '/Tests/UserResults/' + test.id, text: 'User results' });
    td.append(userResultsRef);
    td.append('<span> | </span>');
    var detailsRef = $('<a/>', { href: '/Tests/Details/' + test.id, text: 'Details' });
    td.append(detailsRef);
    tr.append(td);
}