class TestEngine {
    constructor(question) {
        this.currentQuestion = question;
        this.testQuestionsIds = "";
        this.endTime = "";
        this.configureForTheFirstQuestion();
    }

    static startTest() {
        var token = $('input[name="__RequestVerificationToken"]', $('#startTestElem')).val();
        var myData = { code: $("#testCode").val() };
        var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
        var test;
        $.ajax({
            context: this,
            async: false,
            url: "/TestEngine/StartTest",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                test = new TestEngine(data);
            },
            error: function () {
                //$("#questionForm").empty();
            }
        });
        return test;
    }
    configureForTheFirstQuestion() {
        this.getQuestionsId();
        $('#questionBlock').show();
        $('#startTestElem').remove();
        this.appendQuestion();
        this.getUserName();
        this.getEndTime();
        $('#nextQuestionButton').click(function (event) {
            var testQuestionId = $(this).data('nextQuestionId');
            $("button[value='" + testQuestionId + "']").click();
            return false;
        });
        $('#prevQuestionButton').click(function (event) {
            var testQuestionId = $(this).data('prevQuestionId');
            $("button[value='" + testQuestionId + "']").click();
            return false;
        });
    }
    getQuestionsId() {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };

        $.ajax({
            context: this,
            url: "/TestEngine/GetQuestionsIds",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                this.testQuestionsIds = data;
                this.displayQuestionNav();
            },
            error: function () {
                
            }
        });
    }
    getCorrectAnswers() {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };

        $.ajax({
            context: this,
            url: "/TestEngine/GetCorrectAnswers",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                this.displayCorrectAnswers(data);
            },
            error: function () {

            }
        });
    }
    displayCorrectAnswers(questions) {
        let counter = 0;
        questions.forEach(function (elem) {
            let div = $('<div/>', { class: 'correctAnswerBlock' });
            div.append($('<h1/>', { text: ++counter + ') ' + elem.questionText }));

            if (elem.preformattedText != null) {
                var preText = $('<pre/>', { text: elem.preformattedText });
                div.append(preText);
            }

            elem.testAnswers.forEach(function (answer) {
                var input = $('<input />', { type: 'checkbox', name: 'answer', value: answer.id, disabled: 'true' });
                var answerId = answer.id;
                if (elem.userAnswers !== null && elem.userAnswers.indexOf(answerId) !== -1)
                    input.prop({ checked: true });

                var answerText;
                if (answer.isCode)
                    answerText = $('<pre />', { text: answer.answerText });
                else
                    answerText = $('<label />', { text: answer.answerText });

                if (answer.isCorrect)
                    answerText.addClass('text-success');

                var answDiv = $('<div />', { class: "questionAnswer" });
                input.appendTo(answDiv);
                answerText.appendTo(answDiv);
                $('<br>').appendTo(answDiv);
                if (answer.imageName) {
                    var image = $('<img />', { src: '/uploads/answerPics/' + answer.imageName, height: "200", class: 'answerImage' });
                    image.appendTo(answDiv);
                    image.click(function () {
                        if ($(this).hasClass('max')) {
                            $(this).animate({ height: 200 }, 200).removeClass('max');
                        } else {
                            $(this).animate({ height: 600 }, 200).addClass('max');
                        }
                    });
                }
                div.append(answDiv);
            });
            div.appendTo($('#mainContainer'));
        });
    }

    appendQuestion() {
        $('#testQuestionFieldSet div').remove();
        $('#answerButton').removeClass('btn-default');
        $('#answerButton').addClass('btn-success');
        $('#answerButton').text('Answer');

        var h1 = $('<h1/>', { text: this.currentQuestion.questionText });
        $('#question').empty();
        $('#question').css('padding', '0')
        $('#question').append(h1);

        if (this.currentQuestion.preformattedText != null) {
            var preText = $('<pre/>', { text: this.currentQuestion.preformattedText });
            $('#question').append(preText);
        }
            

        $('#testQuestionFieldSet').data('testCode', this.currentQuestion.test.testCode);
        this.currentQuestion.testAnswers.forEach(function (element) {
            var input = $('<input />', { type: 'checkbox', name: 'answer', value: element.id });
            var answer;
            if (element.isCode)
                answer = $('<pre />', { text: element.answerText });
            else
                answer = $('<label />', { text: element.answerText });

            var div = $('<div />', { class: "questionAnswer" });
            input.appendTo(div);
            answer.appendTo(div);
            $('<br>').appendTo(div);
            if (element.imageName) {
                var image = $('<img />', { src: '/uploads/answerPics/' + element.imageName, height: "200", class: 'answerImage' });
                image.appendTo(div);
                image.click(function () {
                    if ($(this).hasClass('max')) {
                        $(this).animate({ height: 200 }, 200).removeClass('max');
                    } else {
                        $(this).animate({ height: 600 }, 200).addClass('max');
                    }
                });
            }
            div.appendTo('#testQuestionFieldSet');
            $("input[type='checkbox']").change(function () {
                var checkButton = $('#answerButton');
                if (checkButton.hasClass('btn-default')) {
                    checkButton.removeClass('btn-default');
                    checkButton.addClass('btn-primary');
                }
            });
        });
        this.setNavButtonsQuestIds();
    }
    getUserName() {
        var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
        var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };
        $.ajax({
            url: "/TestEngine/GetUserName",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                $('#questionText h5').text('Username: ' + data);
            },
            error: function () {
                //$("#questionForm").empty();
            }
        });
    }
    getEndTime() {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };

        $.ajax({
            context: this,
            url: "/TestEngine/GetEndTime",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function(data) {
                this.endTime = data;
                this.timer = startTimer();
            },
            error: function () {
                //$("#questionForm").empty();
            }
        });
    }
    displayQuestionNav() {
        this.setNavButtonsQuestIds();
        for (let i = 0; i < this.testQuestionsIds.length; i++) {
            let button = $('<button />', {
                type: 'button',
                class: 'btn btn-info',
                value: this.testQuestionsIds[i],
                text: (i + 1),
                click: function (event) {
                    navButtonClick(event);
                }
            });
            $('#questions > div > button').first().removeClass('btn-info');
            $('#questions > div > button').first().addClass('btn-primary');
            $('<div />', { class: 'col-xs-1' }).append(button).appendTo('div#questions');
        }
        $('#questions > button:first-child').addClass('btn-primary');
    }
    setNavButtonsQuestIds() {
        var currIdIndex = this.testQuestionsIds.indexOf(this.currentQuestion.id);
        if (currIdIndex - 1 < 0) {
            $('#prevQuestionButton').prop({ disabled: true });
        }
        else {
            $('#prevQuestionButton').prop({ disabled: false });
            $('#prevQuestionButton').data('prevQuestionId', this.testQuestionsIds[currIdIndex - 1]);
        }
        if (currIdIndex + 1 >= this.testQuestionsIds.length) {
            $('#nextQuestionButton').prop({ disabled: true });
        }
        else {
            $('#nextQuestionButton').prop({ disabled: false });
            $('#nextQuestionButton').data('nextQuestionId', this.testQuestionsIds[currIdIndex + 1]);
        }
    }
    checkAnswer() {
        var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
        var checkedArray = new Array();
        $('input[name="answer"]:checked').each(function () {
            checkedArray.push(this.value);
        });

        var myData = { checkedIds: checkedArray };
        myData = $.extend(myData, { 'questionId': this.currentQuestion.id });
        var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });
        $.ajax({
            context: this,
            url: "/TestEngine/CheckAnswer",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                this.showCorrectAnswer(checkedArray);
            },
            error: function () {
                //$("#questionBlock").append('<h5> Internal error. Try again</h5>');
            }
        });
    }
    showCorrectAnswer(userAnswers) {
        userAnswers.forEach(function (element) {
            $("input[type=checkbox][value='" + element + "']").prop('checked', true);
        });

        var navButton = $('button[value="' + this.currentQuestion.id + '"]');
        navButton.removeClass('btn-primary');
        var div = $('<div />', { id: 'answerMessage', class: 'text-center text-primary col-md-offset-3 col-md-2 col-xs-8 col-xs-offset-2' });
        navButton.addClass('btn-default');
        var h3 = $('<h3 />', { class: 'text-center', text: 'Answered' });
        $('#answerButton').text('Change answer');
        $('#answerButton').removeClass('btn-primary');
        $('#answerButton').removeClass('btn-success');
        $('#answerButton').addClass('btn-default');
        div.append(h3);

    }
    getQuestion(questionId) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var myData = { questionId: questionId };
        var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

        $.ajax({
            context: this,
            url: "/TestEngine/GetQuestion",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                this.currentQuestion = data;
                this.appendQuestion();
                this.getIfAnswered();
            },
            error: function () {
                //$("#questionForm").empty();
            }
        });
    }
    getIfAnswered() {
        var token = $('input[name="__RequestVerificationToken"]', $('#questionBlock')).val();
        var questionId = { questionId: this.currentQuestion.id };
        var dataWithAntiforgeryToken = $.extend(questionId, { '__RequestVerificationToken': token });
        $.ajax({
            context: this,
            url: "/TestEngine/GetIfAnswered",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                if (data !== "notAnswered")
                    this.showCorrectAnswer(data);
            },
            error: function () {
                
            }
        });
    }
    finishTest() {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var dataWithAntiforgeryToken = { '__RequestVerificationToken': token };
        $('#timer').remove();
        clearInterval(this.timer);
        $.ajax({
            context: this,
            url: "/TestEngine/FinishTest",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                this.showResult(data);
                if ('isRated' in data) {
                    if (data.isRated)
                        $('#likeButton').addClass('btn-primary');
                    else
                        $('#dislikeButton').addClass('btn-primary');
                }
            },
            error: function () {
                
            }
        });
    }
    showResult(data) {
        $('#questionBlock').remove();
        let link = $('<a />', { href: "/TestResults/Index/" + data.testId, text: "Other users results" });
        $('#mainContainer').append('<h1> Your score: ' + data.score + ' out of ' + this.testQuestionsIds.length + '</h1>');
        this.showRateButtons();
        $('#mainContainer').append(link);
        $('#mainContainer').append($('<br/>'));
        var correctAnswButton = $('<button/>', {
            id: 'correctAnswButton',
            text: 'Show correct answers',
            class: 'btn btn-sm btn-primary',
            click: function (e) {
                showCorrectAnswersClick();
            }
        });
        $('#mainContainer').append(correctAnswButton);
    }
    showRateButtons() {
        let likeButton = $('<button/>', { type: 'button', class: 'btn btn-default btn-sm', id: 'likeButton' });
        likeButton.click(function () {
            rateTestClick(true);
        });
        $('<span/>', { class: 'glyphicon glyphicon-thumbs-up', test: 'Like' }).appendTo(likeButton);
        let dislikeButton = $('<button/>', { type: 'button', class: 'btn btn-default btn-sm', id: 'dislikeButton' });
        dislikeButton.click(function () {
            rateTestClick(false);
        });
        $('<span/>', { class: 'glyphicon glyphicon-thumbs-down', test: 'Dislike' }).appendTo(dislikeButton);
        $('#mainContainer').append('<p>Rate this test:</p>')
        $('#mainContainer').append(likeButton);
        $('#mainContainer').append(dislikeButton);
        $('#mainContainer').append('<br/>');
    }
    rateTest(mark) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var myData = { mark: mark };
        var dataWithAntiforgeryToken = $.extend(myData, { '__RequestVerificationToken': token });

        $.ajax({
            url: "/TestEngine/RateFinishedTestAjax",
            type: "POST",
            data: dataWithAntiforgeryToken,
            success: function (data) {
                if (data) {
                    $('#dislikeButton').removeClass('btn-primary');
                    $('#likeButton').addClass('btn-primary');
                }
                else {
                    $('#likeButton').removeClass('btn-primary');
                    $('#dislikeButton').addClass('btn-primary');
                }
            },
            error: function () {
               
            }
        });
    }
}
var test;
function startTest() {
    test = TestEngine.startTest();
}
function checkAnswerClick() {
    if ($('input[name="answer"]:checked').length === 0)
        return;
    test.checkAnswer();
}
function finishTestButton() {
    $('#finishTestModal').modal('toggle');
    $('#finishTestModal').on('hidden.bs.modal', function (e) {
        test.finishTest();
    });
}
function navButtonClick(event) {
    event.preventDefault();
    var prevButton = $('#questions > div > button.btn-primary');
    if (!prevButton.hasClass('btn-default'))
        prevButton.addClass('btn-info');

    let clickButton = $(event.target);
    prevButton.removeClass('btn-primary');
    clickButton.removeClass('btn-info');
    clickButton.addClass('btn-primary');
    test.getQuestion(clickButton.attr('value'));
    return false;
}
function startTimer() {
    var countDownDate = new Date(test.endTime).getTime();

    return setInterval(function () {

        var now = new Date().getTime();

        var distance = countDownDate - now;

        var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
        if (minutes < 10)
            minutes = "0" + minutes;
        var seconds = Math.floor((distance % (1000 * 60)) / 1000);
        if (seconds < 10)
            seconds = "0" + seconds;
        $('#timer').text(hours + ":"
            + minutes + ":" + seconds + "s ");

        if (distance <= 500) {
            test.finishTest();
        }
    }, 1000);
}
function rateTestClick(mark) {
    test.rateTest(mark);
}
function showCorrectAnswersClick() {
    $('#correctAnswButton').hide();
    test.getCorrectAnswers();
}
$("#questionBlock").hide();


    
