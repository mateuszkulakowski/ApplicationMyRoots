var lastClickedRect = null;
var userID = null;
var languageID = null;

//zmienne poruszanie węzłem - myszką
var startx = null;
var starty = null;
var dx, dy = null;
var g_element = null;
var main_rect = null;
var oldMatrix = null;
var dragging_element = false;
var mouseoverelement = null;

var mother_new_id = -1;
var father_new_id = -1;
var node_new_id = -1;

$(document).ready(function () {
    
    $('li[class="active"]').removeClass();
    $('#menutree').attr('class', 'active');

    //$("#modal_loading").modal({ backdrop: 'static', keyboard: false }) okienko z loadingiem
    webshims.setOptions('forms-ext', { types: 'date' });
    webshims.polyfill('forms forms-ext');
    $.webshims.formcfg = {
        en: {
            dFormat: '-',
            dateSigns: '-',
            patterns: {
                d: "yy-mm-dd"
            }
        }
    };

    userID = $('#svg_container').attr('data-loggeduser');
    languageID = $('#svg_container').attr('data-language');

    //pobieranie drzewa z api
    $.get("api/HtmlBuilder/GetUserMainTree/" + userID, function (data) {
        $(".tree").html(data);

        //przejmujemy zaznaczony węzeł
        if ($('.tree-element-frames-active').attr('class') != undefined) lastClickedRect = $('.tree-element-frames-active').parent();

        //pobieranie textów dla wstawionych węzłów - etykiety dodające
        addTextsToAllNodes(languageID);
        // dodanie onclicka zaznaczenie na czerwono
        addOnClickToAllNodes(); // click do wszystkich nowo dodanych węzłów
        // pobieranie foto dla każdego węzła
        addImageToAllNodes();
    });

    //pobieranie tranformmatrix
    $.get("api/HtmlBuilder/GetUserMainTreeTransformMatrix/" + userID, function (data) {
    if (data !== null)
        $(".tree").attr('transform', data);
    });

    //unload - do zapisywania aktualnego powiększenia/pomnijeszenia przesunięcia drzewa
    $(window).unload(function () {
        $(".nodeImage").attr("xlink:href", "");
        $.ajax({
            url: 'api/HtmlBuilder/SaveUserMainTree/' + userID,
            data: { 'TreeHtml': $('.tree').html() },
            dataType: "json",
            method: 'POST',
            async: false
        });

        $.ajax({
            url: 'api/HtmlBuilder/SaveUserMainTreeTransformMatrix/' + userID,
            headers: { 'matrix': $('.tree').attr('transform') },
            method: 'GET'
            });
    });

});

    function addImageToAllNodes()
    {
        $('image[class=nodeImage]').each(function (index, element) {
            $.ajax({
                url: 'api/User/getUserImage/' + $(element).parent().attr('id') + "/" + $(element).parent().attr('data-mainuser'),
                method: 'GET',
                success: function (data) {
                    $(element).attr('xlink:href', data);
                }
            });
        });

        $('image[class=editImage]').each(function (index, element) {
            $.ajax({
                url: 'api/User/getEditImage/',
                method: 'GET',
                success: function (data) {
                    $(element).attr('xlink:href', data);
                }
            });
        });

        $('image[class=trashImage]').each(function (index, element) {
            $.ajax({
                url: 'api/User/getTrashImage/',
                method: 'GET',
                success: function (data) {
                    $(element).attr('xlink:href', data);
                }
            });
        });
    }

    var elementCallAddNewNode;

    function addNewNodeToTreeClick(element) {
        setValidationToTrue();
        elementCallAddNewNode = element;
        if ($(element).attr('class') == 'addparents')
        {
            $("#modal_AddingNodeMotherFather").modal('show');
        }
        else
        {
            //pobranie tekstu do nagłówka dodaj członka rodziny
            $.ajax({
                url: 'api/Language/getElementTextInLanguage/' + 43,
                headers: {
                    'languageID': languageID
                },
                method: 'GET',
                success: function (data) {
                    $("#addeditheader").html(data);
                    $("#modal_AddingNode").attr("data-edit", -1);
                    $("#modal_AddingNode").modal('show');
                }
            });
        }

    }

    function successData(data, type, element) // type - "father" async z ojca, "mother" async z matki, "" async z dziecka/partnera
    {
        //                      ################################################################################
        //                      DODAWANIE WĘZŁA - obliczanie pozycji - dodawanie eventów i dodawanie do drzewa --
        //                      ################################################################################

        var x = parseInt($(element).parent().children().first().attr("x"));
        var y = parseInt($(element).parent().children().first().attr("y"));
        var width = parseInt($(element).parent().children().first().attr("width"));
        var height = parseInt($(element).parent().children().first().attr("height"));
        var x_center = x + width / 2;
        var space = 120;
        var spacemother = width / 2.43;
        var spacefather = 100;
        var adding_parents = false;
        var ageLabelX;
        var dateBornValueX;
        var dateDeadValueX;
        var ageValueX;
        var adding_partner = false;
        var adding_child = false;
        var linefathermother = "";
        var linepartners = "";
        var linechilden = "";
        var data_have_right_line = 0;
        var data_have_left_line = 0;
        var data_have_up_line = 0;
        var data_have_childern = 1;

        var parent_current_matrix = $(element).parent().attr('transform');
        parent_current_matrix = parent_current_matrix.substring(7, parent_current_matrix.length - 1).split(' ');
        var matrix_parent_x = parent_current_matrix[4];
        var matrix_parent_y = parent_current_matrix[5];

        //pozycje nowego węzła
        var x_new = null;
        var y_new = null;

        var x_new_father = null;
        var y_new_father = null;

        //aktualne przesunięcie danego węzła --
        var matrix = $(element).parent().attr("transform");
        matrix = matrix.substring(7, matrix.length - 1).split(' ');

        //żeby się nie wyświetlało przy dodawaniu np. drugiego ojca
        if ($(element).attr('class') != "addchildren") // dzieci w nieskończoność dodawanie
        {
            if ($(element).attr('class') == "addpartnerR" || $(element).attr('class') == "addpartnerL") // dodanie partnera kasuje obydwie strony
            {
                $(element).parent().children('*[class^=addpartner]').attr('data-have', '1').attr('visibility', 'hidden');
                $(element).parent().children('*[class^=addchildren]').attr('data-have', '0').attr('visibility', 'hidden');
                x_new = x + width + space + parseInt(matrix_parent_x);
                y_new = y + parseInt(matrix_parent_y);
                adding_partner = true;

                if (node_new_id != -1)
                {
                    $(element).parent().attr('data-haveRightLine', '1'); //na którym klikneliśmy ma linię z prawej storny (potrzebne do przeciągania szukanie lini)
                    data_have_left_line = 1; //aktuanie tworzony ma linię z lewej strony

                    var left_node_x = x + width + parseFloat(matrix[4]);// to ten na którego kliknelismy
                    var left_node_y = y + (height / 2) + + parseFloat(matrix[5]);

                    var right_node_x = x_new; // to nasz aktualny dodawany
                    var right_node_y = y_new + (parseInt(height) / 2);

                    var quadratic_x = (parseInt(left_node_x) + parseInt(right_node_x)) / 2;
                    var quadratic_y = ((parseInt(left_node_y) + parseInt(right_node_y)) / 2) + 150;


                    linepartners = "<path  d=\"M" + left_node_x + " " + left_node_y + " Q" + quadratic_x + " " + quadratic_y + " " + right_node_x + " " + right_node_y + "\" fill=\"transparent\" stroke=\"black\" data-left=\"" + $(element).parent().attr('id') + "\" data-right=\"" + node_new_id + "\"/>";
                    node_new_id = -1;
                }
            }
            else // matka/ojciec
            {
                //$(element).parent().children('*[class^=addparents]').attr('data-have', '1').attr('visibility', 'hidden'); // wyłączenie opcji dodawania -- przeniesione do metody addNewNodeToTree()
                if (type=="mother") // matka
                {
                    //pozycja okna matki -- !!! przy zmianie x/y zmienić punkt przyczepienia linii matka/ojciec
                    x_new = x_center + spacemother + parseInt(matrix_parent_x);
                    y_new = y - height - 30 + parseInt(matrix_parent_y);
                    adding_parents = true;
                    data_have_left_line = 1;

                }
                else // ojciec
                {
                    //pozycja okna ojca
                    x_new = x_center - width - spacefather + parseInt(matrix_parent_x);
                    y_new = y - height - 30 + parseInt(matrix_parent_y);
                    adding_parents = true;
                    data_have_right_line = 1;
                }

                // jeśli wykonały się dwa asynki możemy dodać już linię
                if (mother_new_id != -1 && father_new_id != -1) {

                    $(element).parent().prop('data-haveUpLine', '1'); //na którym klikneliśmy ustawiamy że ma linię z góry

                    //połączenie linii z matką czyli połowa długości boku lewego / ojca odwrotnie
                    var mother_line_join_x = x_center + spacemother + parseInt(matrix_parent_x);
                    var mother_line_join_y = y - height - 30 + parseInt(matrix_parent_y) + (parseInt(height) / 2);

                    var father_line_join_x = x_center - width - spacefather + parseInt(matrix_parent_x) + parseInt(width);
                    var father_line_join_y = y - height - 30 + parseInt(matrix_parent_y) + (parseInt(height) / 2);

                    var quadratic_x = (parseInt(father_line_join_x) + parseInt(mother_line_join_x)) / 2;
                    var quadratic_y = ((parseInt(father_line_join_y) + parseInt(mother_line_join_y)) / 2) + 150;

                    // dodajemy połączenie matka-ojciec
                    linefathermother = "<path  d=\"M" + father_line_join_x + " " + father_line_join_y + " Q" + quadratic_x + " " + quadratic_y + " " + mother_line_join_x + " " + mother_line_join_y + "\" fill=\"transparent\" stroke=\"black\" data-left=\"" + father_new_id + "\" data-right=\"" + mother_new_id + "\"/>";

                    var t = 0.5; // połowa długości punkt doczepienia dzieci
                    var x2 = (1 - t) * (1 - t) * parseFloat(father_line_join_x) + 2 * (1 - t) * t * parseFloat(quadratic_x) + t * t * parseFloat(mother_line_join_x);
                    var y2 = (1 - t) * (1 - t) * parseFloat(father_line_join_y) + 2 * (1 - t) * t * parseFloat(quadratic_y) + t * t * parseFloat(mother_line_join_y);

                    var x1 = x + (width / 2) + parseInt(matrix_parent_x);
                    var y1 = y + parseInt(matrix_parent_y);

                    linechilden = "<line x1=\"" + x1 + "\" x2=\"" + x2 + "\" y1=\"" + y1 + "\" y2=\"" + y2 + "\" stroke=\"black\" data-right-node=\"" + mother_new_id + "\" data-left-node=\"" + father_new_id + "\" data-down-node=\"" + $(element).parent().attr('id') + "\"/>";

                    mother_new_id = -1;
                    father_new_id = -1;
                }

            }
        }
        else
        {
            x_new = x + parseInt(matrix_parent_x);
            y_new = y + height + space + parseInt(matrix_parent_y);
            adding_child = true;

            if (node_new_id != -1) {
                data_have_up_line = 1; //aktuanie tworzony ma linię z góry

                var left_node_x = x + width + parseFloat(matrix[4]);// to ten na którego kliknelismy
                var left_node_y = y + (height / 2) + + parseFloat(matrix[5]);

                var right_node_x = x_new; // to nasz aktualny dodawany
                var right_node_y = y_new + (parseInt(height) / 2);

                var quadratic_x = (parseInt(left_node_x) + parseInt(right_node_x)) / 2;
                var quadratic_y = ((parseInt(left_node_y) + parseInt(right_node_y)) / 2) + 150;

                var x1 = x_new + (width / 2);
                var y1 = y_new;

                //do x2,y2 musimy poszukać path-u należącego do danego węzła może to być data-rght lub left
                var d;
                var pathRight;
                var pathLeft;
                if ($(element).parent().attr('data-haveRightLine') == 1)
                {
                    d = $('path[data-left=' + $(element).parent().attr('id') + ']').attr('d');
                    pathRight = $('path[data-left=' + $(element).parent().attr('id') + ']').attr('data-right');
                    pathLeft = $('path[data-left=' + $(element).parent().attr('id') + ']').attr('data-left');
                }
                if ($(element).parent().attr('data-haveLeftLine') == 1)
                {
                    d = $('path[data-right=' + $(element).parent().attr('id') + ']').attr('d');
                    pathRight = $('path[data-right=' + $(element).parent().attr('id') + ']').attr('data-right');
                    pathLeft = $('path[data-right=' + $(element).parent().attr('id') + ']').attr('data-left');
                }
                var dsplit = d.split(' ');

                var t = 0.5; // połowa długości punkt doczepienia dzieci
                var x2 = (1 - t) * (1 - t) * parseFloat(dsplit[0].substr(1)) + 2 * (1 - t) * t * parseFloat(dsplit[2].substr(1)) + t * t * parseFloat(dsplit[4]);
                var y2 = (1 - t) * (1 - t) * parseFloat(dsplit[1]) + 2 * (1 - t) * t * parseFloat(dsplit[3]) + t * t * parseFloat(dsplit[5]);

                linechilden = "<line x1=\"" + x1 + "\" x2=\"" + x2 + "\" y1=\"" + y1 + "\" y2=\"" + y2 + "\" stroke=\"black\" data-right-node=\"" + pathRight + "\" data-left-node=\"" + pathLeft + "\" data-down-node=\"" + data +"\"/>";
                node_new_id = -1;

            }
        }

        //obliczanie x-y textów translatowanych o 90st.
        var parentL_rect_x = parseInt(x_new - 20);
        var parentR_rect_x = parseInt(x_new + 200);

        // y zależy od x_recta musimy wziac jego odwrotność i odjąć 12
        // x zależy od y_recta musimy dodać tylko 40 do wartości
        var inverse_parentR_rect_x = 0; //wyliczenia pozycji zrotowanego napisu o 90 stopni nawiązując recta w którym sie znajduje
        if (parentR_rect_x > 0)
            inverse_parentR_rect_x = (parentR_rect_x - 2 * parentR_rect_x) - 12;
        if (parentR_rect_x < 0)
            inverse_parentR_rect_x = (parentR_rect_x - parentR_rect_x - parentR_rect_x) - 12;

        var inverse_parentL_rect_x = 0; //wyliczenia pozycji zrotowanego napisu o 90 stopni nawiązując recta w którym sie znajduje
        if (parentL_rect_x > 0)
            inverse_parentL_rect_x = (parentL_rect_x - 2 * parentL_rect_x) - 12;
        if (parentL_rect_x < 0)
            inverse_parentL_rect_x = (parentL_rect_x - parentL_rect_x - parentL_rect_x) - 12;

        //napisy angielskie troche inne wartości x/y
        if (languageID == 1)//polski
        {
            var ageLabelX = parseInt(x_new + 47.5);
            dateBornValueX = parseInt(x_new + 70);
            dateDeadValueX = parseInt(x_new + 74);
            ageValueX = parseInt(x_new + 64);
        }
        else {
            var ageLabelX = parseInt(x_new + 47.5);
            dateBornValueX = parseInt(x_new + 70);
            dateDeadValueX = parseInt(x_new + 73);
            ageValueX = parseInt(x_new + 58);
        }

        //ustawianie data_have przy partnerach na 1 przy dodawaniu rodziców bo mają już partnera
        var data_have_partner = 0;
        var data_have_parents = 0;
        if (adding_parents || adding_partner) {
            data_have_partner = 1;
            data_have_childern = 0;
        }

        if (adding_child) {
            data_have_parents = 1;
        }


        //data-main user - w zależności któa tabelka
        var newNode =
            "<g class=\"tree-elements\" id=\"" + data + "\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"0\" data-haveRightLine=\"" + data_have_right_line + "\" data-haveLeftLine=\"" + data_have_left_line + "\" data-haveUpLine=\"1\">" +
                "<rect class=\"tree-element-frames\" width=\"" + width + "\" height=\"" + height + "\" x=\"" + x_new + "\" y=\"" + y_new + "\" fill=\"white\" stroke=\"black\"/>" +
                "<text class=\"tree-element-texts\" x=\"" + parseInt(x_new + 100) + "\" y=\"" + parseInt(y_new + 15) + "\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"" + parseInt(x_new + 10) + "\" y=\"" + parseInt(y_new - 20) + "\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"" + data_have_parents + "\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addparents-dbt\" x=\"" + parseInt(x_new + 100) + "\" y=\"" + parseInt(y_new - 10) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"" + data_have_parents + "\"></text>" +
                "<rect class=\"addpartnerR\" width=\"20\" height=\"80\" x=\"" + parseInt(x_new + 200) + "\" y=\"" + parseInt(y_new + 10) + "\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"" + data_have_partner + "\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addpartnerR-dbt\" transform=\"rotate(90)\" x=\"" + parseInt(y_new + 10 + 40) + "\" y=\"" + inverse_parentR_rect_x + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"22\" data-have=\"" + data_have_partner + "\"></text>" +
                "<rect class=\"addpartnerL\" width=\"20\" height=\"80\" x=\"" + parseInt(x_new - 20) + "\" y=\"" + parseInt(y_new + 10) + "\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"" + data_have_partner + "\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addpartnerL-dbt\" transform=\"rotate(90)\" x=\"" + parseInt(y_new + 10 + 40) + "\" y=\"" + inverse_parentL_rect_x + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\"text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"23\" data-have=\"" + data_have_partner + "\"></text>" +
                "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"" + parseInt(x_new + 10) + "\" y=\"" + parseInt(y_new + 100) + "\" fill=\"white\" stroke=\"#428bca\" visibility=\"hidden\" data-have=\"" + data_have_childern + "\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addchildren-dbt\" x=\"" + parseInt(x_new + 100) + "\" y=\"" + parseInt(y_new + 110) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"" + data_have_childern + "\"></text>" +
                "<image class=\"nodeImage\" xlink:href=\"\" x=\"" + parseInt(x_new + 130) + "\" y=\"" + parseInt(y_new + 30) + "\" height=\"60\" width=\"60\"/>" +
                "<image class=\"editImage\" xlink:href=\"\" x=\"" + parseInt(x_new + 2) + "\" y=\"" + parseInt(y_new + 85) + "\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickEdit(this)\"/>" +
            "<rect class=\"editImageBorder\" x=\"" + parseInt(x_new + 2) + "\" y=\"" + parseInt(y_new + 85) + "\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                "<image class=\"trashImage\" xlink:href=\"\" x=\"" + parseInt(x_new + 20) + "\" y=\"" + parseInt(y_new + 85) + "\" height=\"13\" width=\"13\" visibility=\"hidden\" onclick=\"onClickDelete(this)\"/>" +
            "<rect class=\"trashImageBorder\" x=\"" + parseInt(x_new + 20) + "\" y=\"" + parseInt(y_new + 85) + "\" height=\"13\" width=\"13\" fill=\"none\" stroke=\"#428bca\" stroke-width=\"0.2\" visibility=\"hidden\"/>" +
                "<text class=\"datebirthLabel-dbt\" x=\"" + parseInt(x_new + 38) + "\" y=\"" + parseInt(y_new + 50) + "\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"29\"></text>" +
                "<text class=\"datedeadLabel-dbt\" x=\"" + parseInt(x_new + 38) + "\" y=\"" + parseInt(y_new + 60) + "\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"30\"></text>" +
                "<text class=\"ageLabel-dbt\" x=\"" + ageLabelX + "\" y=\"" + parseInt(y_new + 70) + "\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"31\"></text>" +
                "<text class=\"maininfoLabel-dbt\" x=\"" + parseInt(x_new + 65) + "\" y=\"" + parseInt(y_new + 36) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" data-tag=\"32\"></text>" +
                "<text class=\"datebirthValue\" x=\"" + dateBornValueX + "\" y=\"" + parseInt(y_new + 50) + "\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                "<text class=\"datedeadValue\" x=\"" + dateDeadValueX + "\" y=\"" + parseInt(y_new + 60) + "\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
                "<text class=\"ageValue\" x=\"" + ageValueX + "\" y=\"" + parseInt(y_new + 70) + "\" font-family=\"verdana\" font-size=\"5\" fill=\"grey\" alignment-baseline=\"middle\" text-anchor=\"middle\"></text>" +
            "</g>";

        $(".tree").html($(".tree").html() + newNode);

        if (linefathermother != "")
        {
            $(".tree").html(linefathermother + $(".tree").html());
        }

        if (linepartners != "")
        {
            $(".tree").html(linepartners + $(".tree").html());
        }

        if (linechilden != '')
        {
            $(".tree").html(linechilden + $(".tree").html());
        }


        addOnClickToAllNodes();
        addImageToAllNodes();
        addTextsToAllNodes(languageID); // dodanie tekstów dla nowych węzłów
    }


    function addNewNodeToTree(element, caller) // caller - 1 (dodawanie dziecka/partenra), - 2 (dodawanie ojca/matki)
    { 
        if (caller == 1) // bierzemy z okna o id: modal_AddingNode
        {
            var filesList = $('input[name=file]').prop('files');
            var file = filesList[0];
            if (file != null)
            {
                var data = new FormData();
                data.append('file',file);
                $.ajax({
                    url: 'api/User/SaveUserNode/' + userID,
                    headers: {
                        'withFile': '1',
                        'name': nameValue,
                        'surname': surnameValue,
                        'dateborn': datebornValue,
                        'datedead': datedeadValue,
                        'additionalinfo': additionalinfoValue
                    },
                    processData: false,
                    contentType: false,
                    data: data,
                    type: 'POST',
                    success: function (data) {
                        node_new_id = data;
                        successData(data, "", element);
                    }
                });

            }
            else { // plik zostaje jaki był..
                $.ajax({
                    url: 'api/User/SaveUserNode/' + userID,
                    headers: {
                        'withFile': '0',
                        'name': nameValue,
                        'surname': surnameValue,
                        'dateborn': datebornValue,
                        'datedead': datedeadValue,
                        'additionalinfo': additionalinfoValue
                    },
                    type: 'POST',
                    success: function (data) {
                        node_new_id = data;
                        successData(data, "", element);
                    }
                });
            }
        }
        else // branie danych z okna modalego dodawania matki/ojca - 2
        {
            var filesListMother = $('input[name=filemother]').prop('files');
            var filemother = filesListMother[0];

            var filesListFather = $('input[name=filefather]').prop('files');
            var filefather = filesListFather[0];

            if (filemother != null) {
                var data = new FormData();
                data.append('file', filemother);
                $.ajax({
                    url: 'api/User/SaveUserNode/' + userID,
                    headers: {
                        'withFile': "1",
                        'name': nameMotherValue,
                        'surname': surnameMotherValue,
                        'dateborn': datebornMotherValue,
                        'datedead': datedeadMotherValue,
                        'additionalinfo': additionalinfoMotherValue,
                    },
                    processData: false,
                    contentType: false,
                    data: data,
                    type: 'POST',
                    success: function (data) {
                        mother_new_id = data;
                        successData(data, "mother", element);
                    }
                });
            }
            else {
                $.ajax({
                    url: 'api/User/SaveUserNode/' + userID,
                    headers: {
                        'withFile': "0",
                        'name': nameMotherValue,
                        'surname': surnameMotherValue,
                        'dateborn': datebornMotherValue,
                        'datedead': datedeadMotherValue,
                        'additionalinfo': additionalinfoMotherValue,
                    },
                    type: 'POST',
                    success: function (data) {
                        mother_new_id = data;
                        successData(data, "mother", element);
                    }
                });
            }

            if (filefather != null) {
                var data = new FormData();
                data.append('file', filefather);
                $.ajax({
                    url: 'api/User/SaveUserNode/' + userID,
                    headers: {
                        'withFile': "1",
                        'name': nameFatherValue,
                        'surname': surnameFatherValue,
                        'dateborn': datebornFatherValue,
                        'datedead': datedeadFatherValue,
                        'additionalinfo': additionalinfoFatherValue,
                    },
                    processData: false,
                    contentType: false,
                    data: data,
                    type: 'POST',
                    success: function (data) {
                        father_new_id = data;
                        successData(data, "father", element);
                    }
                });
            }
            else {
                $.ajax({
                    url: 'api/User/SaveUserNode/' + userID,
                    headers: {
                        'withFile': "0",
                        'name': nameFatherValue,
                        'surname': surnameFatherValue,
                        'dateborn': datebornFatherValue,
                        'datedead': datedeadFatherValue,
                        'additionalinfo': additionalinfoFatherValue,
                    },
                    type: 'POST',
                    success: function (data) {
                        father_new_id = data;
                        successData(data, "father", element);
                    }
                });

            }

            $(element).parent().children('*[class^=addparents]').attr('data-have', '1').attr('visibility', 'hidden'); // wyłączenie opcji dodawania 
        }

        setValidationToTrue(); // czyszczenie okna modalnego
    }

    function getEditedData(editid)
    {
        $.ajax({
            url: 'api/User/getUserImage/' + editid + "/" + 0,
            method: 'GET',
            success: function (data) {
                $('#' + editid).children('.nodeImage').attr('xlink:href', data);
            }
        });

        $.get("api/User/getUserDataToNode/" + editid + "/" + 0 + "/" + languageID, function (data) {
            if (data != null) {
                var dataTab = data.split(',');
                $('#' + editid).children("text[class='tree-element-texts']").html(dataTab[0]); //nazwa węzła
                $('#' + editid).children("text[class='datebirthValue']").html(dataTab[1]); //data urodzenia węzła
                $('#' + editid).children("text[class='datedeadValue']").html(dataTab[2]); //data śmierci węzła
                $('#' + editid).children("text[class='ageValue']").html(dataTab[3]); //wiek węzła

                if (languageID == 1) //wyrównywanie napisów
                {
                    $('#' + editid).children("text[class='datebirthValue']").attr("x", parseInt($('#' + editid).children("text[class='datebirthLabel-dbt']").attr("x")) + 42);
                    $('#' + editid).children("text[class='datedeadValue']").attr("x", parseInt($('#' + editid).children("text[class='datedeadLabel-dbt']").attr("x")) + 42);
                    $('#' + editid).children("text[class='ageValue']").attr("x", parseInt($('#' + editid).children("text[class='ageLabel-dbt']").attr("x")) + 33);

                } else {
                    $('#' + editid).children("text[class='datebirthValue']").attr("x", parseInt($('#' + editid).children("text[class='datebirthLabel-dbt']").attr("x")) + 35);
                    $('#' + editid).children("text[class='datedeadValue']").attr("x", parseInt($('#' + editid).children("text[class='datedeadLabel-dbt']").attr("x")) + 35);
                    $('#' + editid).children("text[class='ageValue']").attr("x", parseInt($('#' + editid).children("text[class='ageLabel-dbt']").attr("x")) + 28);
                }
            }
        });
    }


    function editNode(editid)
    {
        var filesList = $('input[name=file]').prop('files');
        var file = filesList[0];
        if (file != null) {
            var data = new FormData();
            data.append('file', file);
            $.ajax({
                url: 'api/User/EditUserNode/' + editid,
                headers: {
                    'withFile': '1',
                    'name': nameValue,
                    'surname': surnameValue,
                    'dateborn': datebornValue,
                    'datedead': datedeadValue,
                    'additionalinfo': additionalinfoValue
                },
                processData: false,
                contentType: false,
                data: data,
                type: 'POST',
                success: function (data) {
                    getEditedData(editid);
                }
            });

        }
        else { // plik zostaje jaki był..
            $.ajax({
                url: 'api/User/EditUserNode/' + editid,
                headers: {
                    'withFile': '0',
                    'name': nameValue,
                    'surname': surnameValue,
                    'dateborn': datebornValue,
                    'datedead': datedeadValue,
                    'additionalinfo': additionalinfoValue
                },
                type: 'POST',
                success: function (data) {
                    getEditedData(editid);
                }
            });
        }

        $('image[id=modalImageNode]').each(function (index, element) {
            $.ajax({
                url: 'api/User/getDefaultImage/',
                method: 'GET',
                success: function (data) {
                    $(element).attr('xlink:href', data);
                }
            });
        });

        setValidationToTrue();
    }

    function mousedowntree(evt)
    {
        var target = $(evt.target);

        startx = evt.clientX;
        starty = evt.clientY;
        $(target).on('mousemove', function (evt) {

            var divider = $(".tree").attr("transform");
            divider = divider.substring(7, divider.length - 1).split(' ');

            dx = (evt.clientX - startx); /// divider[0];
            dy = (evt.clientY - starty); /// divider[0];

            move(dx,dy);

            startx = evt.clientX;
            starty = evt.clientY;
        });

        $(target).on('mouseup', function () {
            if (target != null) {
                $(target).off("mousemove");
                $(target).off("mouseleave");
                $(target).off("mouseup");
                startx, starty, dx, dy, g_element, target = null;
            }
        });

        $(target).on('mouseleave', function (evt) {
            if (target != null) {
                $(target).off("mousemove");
                $(target).off("mouseleave");
                $(target).off("mouseup");
                startx, starty, dx, dy, g_element, target = null;
            }
        });
    }




    function mousedowntreeelement(evt)
    {
        var e = evt;
        g_element = $(evt.target).parent();
        main_rect = $(evt.target);

        startx = evt.clientX;
        starty = evt.clientY;
        oldMatrix = $(g_element).attr("transform");
        oldMatrix = oldMatrix.substring(7, oldMatrix.length - 1).split(' ');
        
        for (var i = 0; i < oldMatrix.length; i++) {
            oldMatrix[i] = parseFloat(oldMatrix[i]);
        }

        $(main_rect).on('mousemove', function (evt) {
            dragging_element = true;

            $(g_element).appendTo(".tree");

            //włączenie kursora do przesuwania
            main_rect.css('cursor', 'move');

            var divider = $(".tree").attr("transform");
            divider = divider.substring(7, divider.length - 1).split(' ');

            dx = evt.clientX - startx;
            dy = evt.clientY - starty;
            oldMatrix[4] += dx / divider[0]; // podzielić na matrix svg
            oldMatrix[5] += dy / divider[0];
            
            var newMatrix = "matrix(" + oldMatrix.join(' ') + ")";
            $(g_element).attr('transform', newMatrix);

            //przeciąganie linii która jest z prawej strony węzła - taka sytuacja tylko w ojcu
            if ($(g_element).attr('data-haveRightLine') == 1) {
                var d = $('path[data-left=' + $(g_element).attr('id') + ']').attr('d');

                var dsplit = d.split(' ');

                var new_posX = oldMatrix[4] + parseInt($(g_element).children('rect').first().attr('x')) + parseInt($(g_element).children('rect').first().attr('width'));
                var new_posY = oldMatrix[5] + parseInt($(g_element).children('rect').first().attr('y')) + (parseInt($(g_element).children('rect').first().attr('height')) / 2);

                var new_posQX = (new_posX + parseInt(dsplit[4])) / 2;
                var new_posQY = ((new_posY + parseInt(dsplit[5])) / 2) + 150;

                $('path[data-left=' + $(g_element).attr('id') + ']').attr('d', 'M' + new_posX + ' ' + new_posY + ' Q' + new_posQX + ' ' + new_posQY + ' ' + dsplit[4] + ' ' + dsplit[5]);

                var t = 0.5; // połowa długości punkt doczepienia dzieci


                //ruszanie liniami dzieci punkt zaczepienia wsyzstkich linii --
                $('line[data-left-node="' + $(g_element).attr('id') + '"]').attr('x2', (1 - t) * (1 - t) * new_posX + 2 * (1 - t) * t * new_posQX + t * t * parseFloat(dsplit[4]))
                $('line[data-left-node="' + $(g_element).attr('id') + '"]').attr('y2', (1 - t) * (1 - t) * new_posY + 2 * (1 - t) * t * new_posQY + t * t * parseFloat(dsplit[5]));
            }

            //przeciąganie końca linii matka ---
            if ($(g_element).attr('data-haveLeftLine') == 1) {

                var d = $('path[data-right=' + $(g_element).attr('id') + ']').attr('d');
                var dsplit = d.split(' ');

                var new_posX = oldMatrix[4] + parseInt($(g_element).children('rect').first().attr('x'));
                var new_posY = oldMatrix[5] + parseInt($(g_element).children('rect').first().attr('y')) + (parseInt($(g_element).children('rect').first().attr('height')) / 2);

                var new_posQX = (new_posX + parseInt(dsplit[0].substr(1))) / 2;
                var new_posQY = ((new_posY + parseInt(dsplit[1])) / 2) + 150;

                $('path[data-right=' + $(g_element).attr('id') + ']').attr('d', dsplit[0] + ' ' + dsplit[1] + ' Q' + new_posQX + ' ' + new_posQY + ' ' + new_posX + ' ' + new_posY);

                var t = 0.5; // połowa długości punkt doczepienia dzieci

                //ruszanie liniami dzieci punkt zaczepienia wsyzstkich linii --
                $('line[data-right-node="' + $(g_element).attr('id') + '"]').attr('x2', (1 - t) * (1 - t) * dsplit[0].substr(1) + 2 * (1 - t) * t * new_posQX + t * t * new_posX);
                $('line[data-right-node="' + $(g_element).attr('id') + '"]').attr('y2', (1 - t) * (1 - t) * dsplit[1] + 2 * (1 - t) * t * new_posQY + t * t * new_posY);
            }


            //przeciąganie końca linii dziecko czyli tylko <line>
            if ($(g_element).attr('data-haveUpLine') == 1) {

                //$('#kkk').html(oldMatrix[4] + ' ' + parseInt($(g_element).children('rect').first().attr('x')) + ' ' + parseInt($(g_element).children('rect').first().attr('width')) / 2);

                var new_posX = oldMatrix[4] + parseInt($(g_element).children('rect').first().attr('x')) + (parseInt($(g_element).children('rect').first().attr('width')) / 2);
                var new_posY = oldMatrix[5] + parseInt($(g_element).children('rect').first().attr('y'));

                //ruszanie liniami x1y1 czyli ten koniec który jest przyczepiony do dziecka
                $('line[data-down-node="' + $(g_element).attr('id') + '"]').attr('x1', new_posX);
                $('line[data-down-node="' + $(g_element).attr('id') + '"]').attr('y1', new_posY);
            }

            startx = evt.clientX;
            starty = evt.clientY;
        });

        $(main_rect).on('mouseup', function () {
            if (g_element != null && main_rect != null) {
                main_rect.css('cursor', 'default');
                $(main_rect).off("mousemove");
                $(main_rect).off("mouseout");
                $(main_rect).off("mouseup");
                startx, starty, dx, dy, g_element, main_rect = null;
            }
        });


        $(main_rect).on('mouseout', function (e) {
            if (g_element != null && main_rect != null) {
                main_rect.css('cursor', 'default');
                $(main_rect).off("mousemove");
                $(main_rect).off("mouseout");
                $(main_rect).off("mouseup");
                startx, starty, dx, dy, g_element, main_rect = null;
            }
        });
        
    }

    function addTextsToAllNodes(languageID)
    {
        $('g[class="tree-elements"]').each(function (index, element) {
            $.get("api/User/getUserDataToNode/" + $(element).attr('id') + "/" + $(element).attr('data-mainuser') + "/" + languageID, function (data) {

                if (data != null)
                {
                    var dataTab = data.split(',');
                    $(element).children("text[class='tree-element-texts']").html(dataTab[0]); //nazwa węzła
                    $(element).children("text[class='datebirthValue']").html(dataTab[1]); //data urodzenia węzła
                    $(element).children("text[class='datedeadValue']").html(dataTab[2]); //data śmierci węzła
                    $(element).children("text[class='ageValue']").html(dataTab[3]); //wiek węzła

                    if (languageID == 1) //wyrównywanie napisów
                    {
                        $(element).children("text[class='datebirthValue']").attr("x", parseInt($(element).children("text[class='datebirthLabel-dbt']").attr("x")) + 42);
                        $(element).children("text[class='datedeadValue']").attr("x", parseInt($(element).children("text[class='datedeadLabel-dbt']").attr("x")) + 42);
                        $(element).children("text[class='ageValue']").attr("x", parseInt($(element).children("text[class='ageLabel-dbt']").attr("x")) + 33);

                    } else {
                        $(element).children("text[class='datebirthValue']").attr("x", parseInt($(element).children("text[class='datebirthLabel-dbt']").attr("x")) + 35);
                        $(element).children("text[class='datedeadValue']").attr("x", parseInt($(element).children("text[class='datedeadLabel-dbt']").attr("x")) + 35);
                        $(element).children("text[class='ageValue']").attr("x", parseInt($(element).children("text[class='ageLabel-dbt']").attr("x")) + 28);
                    }
                    
                }
            });
        });

        $("text[class$='-dbt']").each(function (index, element) {
            if ($(this).html() != null)
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
    }

    function addOnClickToAllNodes() {
        $('.tree-elements').on('click', function (element) {

            if ($(element.target).attr('class') == 'editImage' ||
                $(element.target).attr('class') == 'trashImage')
                return;

            if (dragging_element)
            {
                dragging_element = false;
                return;
            }

            if (lastClickedRect !== null) {
                lastClickedRect.children('.tree-element-frames-active').attr('stroke', 'black');
                lastClickedRect.children('.tree-element-frames-active').attr('class', 'tree-element-frames');
                //chowanie przycisków do dodawania
                lastClickedRect.children('*[class^=add][data-have=0]').attr('visibility', 'hidden');
                lastClickedRect.children('*[class^=editImage]').attr('visibility', 'hidden');
                lastClickedRect.children('*[class^=trashImage]').attr('visibility', 'hidden');

                if (lastClickedRect.attr('id') === $(this).attr('id')) // gdy chcemy odznaczyć zaznaczonego
                {
                    lastClickedRect = null;
                    return;
                }
            }

            $(this).children('.tree-element-frames').attr('stroke', '#428bca');
            $(this).children('.tree-element-frames').attr('class', 'tree-element-frames-active');
            //wyświetlenie przycisków do dodawania członków rodziny
            $(this).children('*[class^=add][data-have=0]').attr('visibility', 'visible');
            $(this).children('*[class^=editImage]').attr('visibility', 'visible');
            $(this).children('*[class^=trashImage]').attr('visibility', 'visible');

            lastClickedRect = $(this);
            
        });
    }

    function onClickEdit(element)
    {
        setValidationToTrue();

        $.ajax({
            url: 'api/User/GetUserNode/' + $(element).parent().attr('id'),
            type: 'POST',
            success: function (data) {
                if (data != "")
                {
                    var dataSplit = data.split('\\;;/');

                    $('#modalImageNode').attr("xlink:href", $(element).parent().children('.nodeImage').attr("xlink:href"));

                    if (dataSplit[0] != "") //name
                    {
                        $('input[name="name"]').val(dataSplit[0]);
                    }
                    if (dataSplit[1] != "") //surname
                    {
                        $('input[name="surname"]').val(dataSplit[1]);
                    }
                    if (dataSplit[2] != "") // date born
                    {
                        $('input[name="dateborn"]').val(dataSplit[2]);
                    }
                    if (dataSplit[3] != "") //date dead
                    {
                        $('input[name="datedead"]').val(dataSplit[3]);
                    }
                    if (dataSplit[4] != "") //additional info
                    {
                        $('textarea[name="additionalinfo"]').val(dataSplit[4]);
                    }
                    if (dataSplit[5] != "") // image
                    {
                        $('#modalImageNode').attr("xlink:href", dataSplit[5]);
                    }

                    //pobranie tekstu edytuj członka rodziny do nagłówka
                    $.ajax({
                        url: 'api/Language/getElementTextInLanguage/' + 68,
                        headers: {
                            'languageID': languageID
                        },
                        method: 'GET',
                        success: function (data) {
                            $("#addeditheader").html(data);
                            $("#modal_AddingNode").attr("data-edit", $(element).parent().attr('id'));
                            $("#modal_AddingNode").modal('show');
                        }
                    });
                }
            }
        });
    }

    var deletingElement = -1;

    function onClickDelete(element)
    {
        deletingElement = element;
        $("#modal_ConfirmDelete").modal('show');
    }


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


    // WALIDACJA OKIEN MODALNYCH // --------------------------------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // --------------------------------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // --------------------------------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // ---------------------V----------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // ---------------------V----------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // ---------------------V----------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // ---------------------V----------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
    // WALIDACJA OKIEN MODALNYCH // --------------------------------------------------<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    var allowExtensions = ['png', 'jpg', 'bmp', 'gif'];
    var fileIsRight = true;
    var nameIsRight = true;
    var surnameIsRight = true;
    var datebornIsRight = true;
    var datedeadIsRight = true;

    var fileIsRightMother = true;
    var nameIsRightMother = true;
    var surnameIsRightMother = true;
    var datebornIsRightMother = true;
    var datedeadIsRightMother = true;

    var fileIsRightFather = true;
    var nameIsRightFather = true;
    var surnameIsRightFather = true;
    var datebornIsRightFather = true;
    var datedeadIsRightFather = true;

    var nameValue;
    var surnameValue;
    var datebornValue;
    var datedeadValue;
    var additionalinfoValue;

    var nameMotherValue;
    var surnameMotherValue;
    var datebornMotherValue;
    var datedeadMotherValue;
    var additionalinfoMotherValue;

    var nameFatherValue;
    var surnameFatherValue;
    var datebornFatherValue;
    var datedeadFatherValue;
    var additionalinfoFatherValue;

    function closeOkModalConfirm()
    {
        if ($(deletingElement).parent().attr("data-haveRightLine") == 1 ||
            $(deletingElement).parent().attr("data-haveLeftLine") == 1) //ma partnera
        {
            var partnerID;
            var pathToRemove;
            var partnerIsLeft;

            if ($('path[data-left=' + $(deletingElement).parent().attr('id') + ']').attr('data-right') != undefined)
            {
                pathToRemove = $('path[data-left=' + $(deletingElement).parent().attr('id') + ']');
                partnerID = $('path[data-left=' + $(deletingElement).parent().attr('id') + ']').attr('data-right');
                partnerIsLeft = false;
            } 
            else
            {
                pathToRemove = $('path[data-right=' + $(deletingElement).parent().attr('id') + ']');
                partnerID = $('path[data-right=' + $(deletingElement).parent().attr('id') + ']').attr('data-left');
                partnerIsLeft = true;
            }

            var size;
            var childrenID;

            if (partnerIsLeft) //liczba dzieci z partnerem jak 1 można usuwać jak wiecej błąd
            {
                size = $('line[data-right-node="' + $(deletingElement).parent().attr('id') + '"][data-left-node="' + partnerID + '"]').length;
                childrenID = $('line[data-right-node="' + $(deletingElement).parent().attr('id') + '"][data-left-node="' + partnerID + '"]').attr('data-down-node');
            }
            else
            {
                size = $('line[data-left-node="' + $(deletingElement).parent().attr('id') + '"][data-right-node="' + partnerID + '"]').length;
                childrenID = $('line[data-left-node="' + $(deletingElement).parent().attr('id') + '"][data-right-node="' + partnerID + '"]').attr('data-down-node');
            }
            
            if (size > 1) // wyrzucamy błąd jak ma wiecej niż jedno dziecko
            {
                $("#modal_ErrorDeleting").modal("show");
                $("#modal_ConfirmDelete").modal("hide");
                return;
            }

            if (size == 1)
            {
                var childrenpartnerID;

                //dziecko nie ma partnera nie możemy usunąć
                if ($('path[data-left=' + childrenID + ']').attr('data-right') != undefined && $('path[data-right=' + childrenID + ']').attr('data-left') != undefined)
                {
                    $("#modal_ErrorDeleting").modal("show");
                    $("#modal_ConfirmDelete").modal("hide");
                    return;
                }

                //dziecko ma partnera jeżeli partner to użytkownik możemy usunąć
                if ($('path[data-left=' + childrenID + ']').attr('data-right') != undefined)
                {
                    childrenpartnerID = $('path[data-left=' + childrenID + ']').attr('data-right');
                }
                else
                {
                    childrenpartnerID = $('path[data-right=' + childrenID + ']').attr('data-left');
                }

                if (($('#' + childrenpartnerID).attr('data-mainuser') == 1 && $('line[data-down-node="' + $(deletingElement).parent().attr('id') + '"]').attr('x1') == undefined &&
                    $('line[data-down-node="' + partnerID + '"]').attr('x1') == undefined) || 
                    ($('line[data-down-node="' + $(deletingElement).parent().attr('id') + '"]').attr('x1') == undefined &&
                     $('line[data-down-node="' + partnerID + '"]').attr('x1') == undefined)) // możemy usuwać rodziców gdy dziecko ma partnera jako uzytkownika, lub partner usuwanego węzła nie ma rodziców
                {
                    $.ajax({
                        url: 'api/User/RemoveUserNode/',
                        headers: {
                            'id1': partnerID,
                            'id2': $(deletingElement).parent().attr('id')
                        },
                        method: 'POST',
                        success: function (data) {
                            $(pathToRemove).remove();
                            $('#' + partnerID).remove();
                            $(deletingElement).parent().remove();
                            $('line[data-down-node="' + childrenID + '"]').remove();
                            $('#' + childrenID).children('text[class^=addparent], rect[class^=addparent]').attr('data-have', '0');
                        }
                    });

                    $("#modal_ConfirmDelete").modal("hide");
                    return;
                }
                else // nie możemy usuwać
                {
                    $("#modal_ErrorDeleting").modal("show");
                    $("#modal_ConfirmDelete").modal("hide");
                    return;
                }
            }

            //patrzymy czy nie jest dzieckiem z góry
            if ($('line[data-down-node="' + $(deletingElement).parent().attr('id') + '"]').attr('x1') != undefined) {
                $("#modal_ErrorDeleting").modal("show");
                $("#modal_ConfirmDelete").modal("hide");
                return;
            }

            if ($('#' + partnerID).attr('data-mainuser') != 0) //jak partnerem węzła usuwanego jesteśmy my - ci którzy mają konto
            {
                //w tym przypadku gdy to jest partner naszego konta nie może być dzieci bo pozostanie bez połączenia te dziecko
                if (size > 0)
                {
                    $("#modal_ErrorDeleting").modal("show");
                    $("#modal_ConfirmDelete").modal("hide");
                    return;
                }

                $.ajax({
                    url: 'api/User/RemoveUserNode/',
                    headers: {
                        'id1': $(deletingElement).parent().attr('id'),
                        'id2': '-1'
                    },
                    method: 'POST',
                    success: function (data) {
                        $(pathToRemove).remove();
                        $(deletingElement).parent().remove();
                        deletingElement = -1;

                        if (partnerIsLeft) {
                            $('#' + partnerID).children('text[class^=addpartner], rect[class^=addpartner]').attr('data-have', '0');
                            $('#' + partnerID).children('text[class^=addchildren], rect[class^=addchildren]').attr('data-have', '1');
                            $('#' + partnerID).attr('data-haveRightLine', '0');
                        }
                        else {
                            $('#' + partnerID).children('text[class^=addpartner], rect[class^=addpartner]').attr('data-have', '0');
                            $('#' + partnerID).children('text[class^=addchildren], rect[class^=addchildren]').attr('data-have', '1');
                            $('#' + partnerID).attr('data-haveLeftLine', '0');
                        }
                    }
                });
            }
            else
            {
                $.ajax({
                    url: 'api/User/RemoveUserNode/',
                    headers: {
                        'id1': partnerID,
                        'id2': $(deletingElement).parent().attr('id')
                    },
                    method: 'POST',
                    success: function (data) {
                        $(pathToRemove).remove();
                        $('#' + partnerID).remove();
                        $(deletingElement).parent().remove();
                        $('line[data-down-node="' + partnerID + '"]').remove();
                    }
                });

                $("#modal_ConfirmDelete").modal("hide");
                return;
            }

            $("#modal_ConfirmDelete").modal("hide");
        }
        else //można usunąć węzeł - jest to samotne dziecko
        {
            $.ajax({
                url: 'api/User/RemoveUserNode/',
                headers: {
                    'id1': $(deletingElement).parent().attr('id'),
                    'id2': '-1'
                },
                method: 'POST',
                success: function (data) {
                    $('line[data-down-node="' + $(deletingElement).parent().attr('id') + '"]').remove();
                    $(deletingElement).parent().remove();
                }
            });

            $("#modal_ConfirmDelete").modal("hide");
        }
    }

    function closeCancelModalConfirm()
    {
        $("#modal_ConfirmDelete").modal("hide");
    }

    function closeCancelModal() {

        $('image[id=modalImageNode]').each(function (index, element) {
            $.ajax({
                url: 'api/User/getDefaultImage/',
                method: 'GET',
                success: function (data) {
                    $(element).attr('xlink:href', data);
                }
            });
        });

        setValidationToTrue();
        $("#modal_AddingNode").modal("hide");
    }

    function closeOkModal() {
        if (fileIsRight && nameIsRight && surnameIsRight && datebornIsRight && datedeadIsRight)
        {
            nameValue = $('input[name=name]').val();
            surnameValue = $('input[name=surname]').val();
            datebornValue = $('input[name=dateborn]').val();
            datedeadValue = $('input[name=datedead]').val();
            additionalinfoValue = $('textarea[name=additionalinfo]').val();

            if ($("#modal_AddingNode").attr("data-edit") == -1)
                addNewNodeToTree(elementCallAddNewNode, 1);
            else
                editNode($("#modal_AddingNode").attr("data-edit"));

            $("#modal_AddingNode").modal("hide");
        }
    }

    function closeCancelModalMotherFather() {

        $('image[id=modalImageNodeMother][id=modalImageNodeFather]').each(function (index, element) {
            $.ajax({
                url: 'api/User/getDefaultImage/',
                method: 'GET',
                success: function (data) {
                    $(element).attr('xlink:href', data);
                }
            });
        });

        setValidationToTrue();
        $("#modal_AddingNodeMotherFather").modal("hide");
    }

    function closeOkModalMotherFather() {
        if (fileIsRightMother && nameIsRightMother && surnameIsRightMother && datebornIsRightMother && datedeadIsRightMother &&
            fileIsRightFather && nameIsRightFather && surnameIsRightFather && datebornIsRightFather && datedeadIsRightFather)
        {
            nameMotherValue = $('input[name=namemother]').val();
            surnameMotherValue = $('input[name=surnamemother]').val();
            datebornMotherValue = $('input[name=datebornmother]').val();
            datedeadMotherValue = $('input[name=datedeadmother]').val();
            additionalinfoMotherValue = $('textarea[name=additionalinfomother]').val();

            nameFatherValue = $('input[name=namefather]').val();
            surnameFatherValue = $('input[name=surnamefather]').val();
            datebornFatherValue = $('input[name=datebornfather]').val();
            datedeadFatherValue = $('input[name=datedeadfather]').val();
            additionalinfoFatherValue = $('textarea[name=additionalinfofather]').val();

            addNewNodeToTree(elementCallAddNewNode, 2);
            $("#modal_AddingNodeMotherFather").modal("hide");
        }
    }

    function setValidationToTrue() {
        fileIsRight = true;
        nameIsRight = true;
        surnameIsRight = true;
        datebornIsRight = true;
        datedeadIsRight = true;
        fileIsRightMother = true;
        nameIsRightMother = true;
        surnameIsRightMother = true;
        datebornIsRightMother = true;
        datedeadIsRightMother = true;
        fileIsRightFather = true;
        nameIsRightFather = true;
        surnameIsRightFather = true;
        datebornIsRightFather = true;
        datedeadIsRightFather = true;

        $('circle[name*="Informator"]').attr('fill', 'green');
        $('input').val(null);
        $('textarea').val("");
    }

    function fileChanged(element) {
        var dialogFile = '';
        if ($(element).attr('name') == 'filefather') // jak matka lub ojciec dodajemy końcówkę nazwy odpowiednio mother/father
            dialogFile = 'father';
        else if ($(element).attr('name') == 'filemother')
            dialogFile = 'mother';

        var isRightName = false;
        var extension = $(element).val().substring($(element).val().lastIndexOf('.') + 1);

        for (var item in allowExtensions) {
            if (allowExtensions[item].toUpperCase() == extension.toUpperCase()) //extension prawidłowe
            {
                isRightName = true;
                break;
            }
        }
        var name = 'fileInformator' + dialogFile;
        if (isRightName) {
            $('circle[name=' + name + ']').attr('fill', 'orange');
            if (dialogFile == 'father')
                fileIsRightFather = true;
            else if (dialogFile == 'mother')
                fileIsRightMother = true;
            else
                fileIsRight = true;
        }
        else {
            $('circle[name=' + name + ']').attr('fill', 'red');
            if (dialogFile == 'father')
                fileIsRightFather = false;
            else if (dialogFile == 'mother')
                fileIsRightMother = false;
            else
                fileIsRight = false;
        }
    }

    function nameChanged(element) {
        var dialogName = '';
        if ($(element).attr('name') == 'namefather') // jak matka lub ojciec dodajemy końcówkę nazwy odpowiednio mother/father
            dialogName = 'father';
        else if ($(element).attr('name') == 'namemother')
            dialogName = 'mother';

        var valueOfInput = $(element).val();
        var name = 'nameInformator' + dialogName;

        if (valueOfInput == "") {
            $('circle[name=' + name + ']').attr('fill', 'red');
            if (dialogName == 'father')
                nameIsRightFather = false;
            else if (dialogName == 'mother')
                nameIsRightMother = false;
            else
                nameIsRight = false;
        }
        else {
            $('circle[name=' + name + ']').attr('fill', 'orange');
            if (dialogName == 'father')
                nameIsRightFather = true;
            else if (dialogName == 'mother')
                nameIsRightMother = true;
            else
                nameIsRight = true;
        }
    }

    function surnameChanged(element) {
        var dialogSurname = '';
        if ($(element).attr('name') == 'surnamefather') // jak matka lub ojciec dodajemy końcówkę nazwy odpowiednio mother/father
            dialogSurname = 'father';
        else if ($(element).attr('name') == 'surnamemother')
            dialogSurname = 'mother';

        var valueOfInput = $(element).val();
        var name = 'surnameInformator' + dialogSurname;

        if (valueOfInput == "") {
            $('circle[name=' + name + ']').attr('fill', 'red');
            if (dialogSurname == 'father')
                surnameIsRightFather = false;
            else if (dialogSurname == 'mother')
                surnameIsRightMother = false;
            else
                surnameIsRight = false;
        }
        else {
            $('circle[name=' + name + ']').attr('fill', 'orange');
            if (dialogSurname == 'father')
                surnameIsRightFather = true;
            else if (dialogSurname == 'mother')
                surnameIsRightMother = true;
            else
                surnameIsRight = true;
        }
    }

    function dateBornChanged(element) {
        var dialogDate = '';
        if ($(element).attr('name') == 'datebornfather') // jak matka lub ojciec dodajemy końcówkę nazwy odpowiednio mother/father
            dialogDate = 'father';
        else if ($(element).attr('name') == 'datebornmother')
            dialogDate = 'mother';

        var d = new Date();
        var yearNow = d.getFullYear();
        var monthNow = d.getMonth() + 1;
        var dayNow = d.getDay() + 1;
        var table = $(element).val().split('-');
        var name = 'datebornInformator' + dialogDate;
        // 1753 - 1 - 1 min baza tyle obsuguje max 9999 12 31
        if (yearNow < table[0] ||
            (yearNow == table[0] && monthNow < table[1]) ||
            (yearNow == table[0] && monthNow == table[1] && dayNow < table[2]) || //urodzeni w przyszłości
            (1752 >= table[0]) || (9999 < table[0])) {
            $('circle[name=' + name + ']').attr('fill', 'red');
            if (dialogDate == 'father')
                datebornIsRightFather = false;
            else if (dialogDate == 'mother')
                datebornIsRightMother = false;
            else
                datebornIsRight = false;
        }
        else {
            $('circle[name=' + name + ']').attr('fill', 'orange');
            if (dialogDate == 'father')
                datebornIsRightFather = true;
            else if (dialogDate == 'mother')
                datebornIsRightMother = true;
            else
                datebornIsRight = true;
        }
    }

    function dateDeadChanged(element) {
        var dialogDate = '';
        if ($(element).attr('name') == 'datedeadfather') // jak matka lub ojciec dodajemy końcówkę nazwy odpowiednio mother/father
            dialogDate = 'father';
        else if ($(element).attr('name') == 'datedeadmother')
            dialogDate = 'mother';

        var d = new Date();
        var yearNow = d.getFullYear();
        var monthNow = d.getMonth() + 1;
        var dayNow = d.getDate();
        var table = $(element).val().split('-');
        var name = 'datedeadInformator' + dialogDate;
        // 1753 - 1 - 1 min baza tyle obsuguje max 9999 12 31
        if (yearNow < table[0] ||
            (yearNow == table[0] && monthNow < table[1]) ||
            (yearNow == table[0] && monthNow == table[1] && dayNow < table[2]) || //w przyszłości
            (1752 >= table[0]) || (9999 < table[0])) {
            $('circle[name=' + name + ']').attr('fill', 'red');
            if (dialogDate == 'father')
                datedeadIsRightFather = false;
            else if (dialogDate == 'mother')
                datedeadIsRightMother = false;
            else
                datedeadIsRight = false;
        }
        else {
            $('circle[name=' + name + ']').attr('fill', 'orange');
            if (dialogDate == 'father')
                datedeadIsRightFather = true;
            else if (dialogDate == 'mother')
                datedeadIsRightMother = true;
            else
                datedeadIsRight = true;
        }
    }

    function additionalChanged(element) {
        var dialogAdditional = '';
        if ($(element).attr('name') == 'additionalinfofather') // jak matka lub ojciec dodajemy końcówkę nazwy odpowiednio mother/father
            dialogAdditional = 'father';
        else if ($(element).attr('name') == 'additionalinfomother')
            dialogAdditional = 'mother';

        var name = 'additionalinfoInformator' + dialogAdditional;
        $('circle[name=' + name + ']').attr('fill', 'orange');
    }