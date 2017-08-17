var lastClickedRect = null;

$(document).ready(function () {
    //$("#modal_loading").modal({ backdrop: 'static', keyboard: false }) okienko z loadingiem

    var userID = $('#svg_container').attr('data-loggeduser');
    var languageID = $('#svg_container').attr('data-language');

    //pobieranie drzewa z api
    $.get("api/HtmlBuilder/GetUserMainTree/" + userID, function (data) {
        $(".tree").html(data);

        //pobieranie textów dla wstawionych węzłów - etykiety dodające
        $("text[class^=add]").each(function (index, element) {
            $.ajax({
                url: 'api/Language/getElementTextInLanguage/' + $(element).attr('data-tag'),
                headers: {
                    'languageID': languageID
                            },
                method: 'GET',
                success: function (data) {
                    $(element).html(data);
                    }
                });

        });

// dodanie onclicka zaznaczenie na czerwono
$('.tree-elements').on('click', function () {

    if (lastClickedRect !== null) {
        lastClickedRect.children('.tree-element-frames-active').attr('stroke', 'black');
        lastClickedRect.children('.tree-element-frames-active').attr('class', 'tree-element-frames');

        //chowanie przycisków do dodawania
        $(this).children('*[class^=add]').attr('visibility', 'hidden');

        if (lastClickedRect.attr('id') === $(this).attr('id')) // gdy chcemy odznaczyć zaznaczonego
        {
            lastClickedRect = null;
            return;
        }
    }

    $(this).children('.tree-element-frames').attr('stroke', 'red');
    $(this).children('.tree-element-frames').attr('class', 'tree-element-frames-active');

    lastClickedRect = $(this);

    //wyświetlenie przycisków do dodawania członków rodziny
    $(this).children('*[class^=add]').attr('visibility', 'visible');

});

// dodanie onclicka na addfather
$('rect[class^=add]').on('click', function () {
    var x = $(this).parent().attr("x");
    var y = $(this).parent().attr("y");
    var width = $(this).parent().attr("width");
    var height = $(this).parent().attr("height");


    alert(x);
});
            });


//pobieranie tranformmatrix
    $.get("api/HtmlBuilder/GetUserMainTreeTransformMatrix/" + userID, function (data) {
    if (data !== null)
        $(".tree").attr('transform', data);
});


//unload - do zapisywania aktualnego powiększenia/pomnijeszenia przesunięcia drzewa
$(window).unload(function () {
    $.ajax({
        url: 'api/HtmlBuilder/SaveUserMainTreeTransformMatrix/' + userID,
            headers: { 'matrix': $('.tree').attr('transform') },
    method: 'GET'
                });
            });

        });

function move(dx, dy) {
    var matrix = $('.tree').attr('transform');
    matrix = matrix.substring(7, matrix.length - 1);
    matrix = matrix.split(' ');

    matrix[4] = parseInt(matrix[4]) + dx;
    matrix[5] = parseInt(matrix[5]) + dy;

    $('.tree').attr('transform', "matrix(" + matrix[0] + ' ' + matrix[1] + ' ' + matrix[2] + ' ' + matrix[3] + ' ' + matrix[4] + ' ' + matrix[5] + ')');
}

function scale(scale) {
    var matrix = $('.tree').attr('transform');
    matrix = matrix.substring(7, matrix.length - 1);
    matrix = matrix.split(' ');

    for (var i = 0; i < matrix.length; i++) {
        matrix[i] *= scale;
    }

    matrix[4] += (1 - scale) * parseInt($('.svg_container').width()) / 2;
    matrix[5] += (1 - scale) * parseInt($('.svg_container').height()) / 2;

    $('.tree').attr('transform', "matrix(" + matrix[0] + ' ' + matrix[1] + ' ' + matrix[2] + ' ' + matrix[3] + ' ' + matrix[4] + ' ' + matrix[5] + ')');
}