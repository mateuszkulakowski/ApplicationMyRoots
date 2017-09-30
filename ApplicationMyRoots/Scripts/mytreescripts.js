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

    $(document).ready(function () {
        //$("#modal_loading").modal({ backdrop: 'static', keyboard: false }) okienko z loadingiem

        userID = $('#svg_container').attr('data-loggeduser');
        languageID = $('#svg_container').attr('data-language');

        //pobieranie drzewa z api
        $.get("api/HtmlBuilder/GetUserMainTree/" + userID, function (data) {
            $(".tree").html(data);
            
            //pobieranie textów dla wstawionych węzłów - etykiety dodające
            addTextsToAddButtonsAllNodes(languageID);
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
    }

    function addNewNodeToTreeClick(element)
    {
        var x = parseInt($(element).parent().children().first().attr("x"));
        var y = parseInt($(element).parent().children().first().attr("y"));
        var width = parseInt($(element).parent().children().first().attr("width"));
        var height = parseInt($(element).parent().children().first().attr("height"));
        var x_center = x + width / 2;
        var space = 15;
        var spacemother = width/2.43;
        var spacefather = 100;
        var if_add_father = false;

        var parent_current_matrix = $(element).parent().attr('transform');
        parent_current_matrix = parent_current_matrix.substring(7, parent_current_matrix.length - 1).split(' ');
        var matrix_parent_x = parent_current_matrix[4];
        var matrix_parent_y = parent_current_matrix[5];

        var partnerData_have = 0;
        var partnerData_have = 0;

        //pozycje nowego węzła
        var x_new = null;
        var y_new = null;

        var x_new_father = null;
        var y_new_father = null;

        //chowanie przycisków do dodawania zaznaczonej osoby
        lastClickedRect.children('.tree-element-frames-active').attr('stroke', 'black');
        lastClickedRect.children('.tree-element-frames-active').attr('class', 'tree-element-frames');
        lastClickedRect.children('*[class^=add][data-have=0]').attr('visibility', 'hidden');
        lastClickedRect = null;

        //żeby się nie wyświetlało przy dodawaniu np. drugiego ojca
        if ($(element).attr('class') != "addchildren") // dzieci w nieskończoność dodawanie
        {
            partnerData_have = 1; // wyłączamy dodanie partnerów

            if ($(element).attr('class') == "addpartnerR" || $(element).attr('class') == "addpartnerL") // dodanie partnera kasuje obydwie strony
            {
                $(element).parent().children('*[class^=addpartner]').attr('data-have', '1').attr('visibility', 'hidden');
                x_new = x + width + space + parseInt(matrix_parent_x);
                y_new = y + parseInt(matrix_parent_y);
            }
            else // matka/ojciec
            {
                if ($(element).attr("class") == "addparents") // dodawanie rodziców
                {
                    $(element).parent().children('*[class=addparents]').attr('data-have', '1').attr('visibility', 'hidden'); // wyłączenie opcji dodawania 

                    //pozycja okna matki
                    x_new = x_center + spacemother + parseInt(matrix_parent_x);
                    y_new = y - height - 30 + parseInt(matrix_parent_y);
                    //pozycja okna ojca
                    x_new_father = x_center - width - spacefather + parseInt(matrix_parent_x);
                    y_new_father = y - height - 30 + parseInt(matrix_parent_y);
                    if_add_father = true;
                }
            }
        }
        else {
            x_new = x + parseInt(matrix_parent_x);
            y_new = y + height + space + parseInt(matrix_parent_y);
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
            
        var newNode =
            "<g class=\"tree-elements\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"0\">" +
                "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"" + x_new + "\" y=\"" + y_new + "\" fill=\"white\" stroke=\"black\"/>" +
                "<text class=\"tree-element-texts\" x=\"" + parseInt(x_new + 100) + "\" y=\"" + parseInt(y_new + 15) + "\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\">Test</text>" +
                "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"" + parseInt(x_new + 10) + "\" y=\"" + parseInt(y_new - 20) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addparents\" x=\"" + parseInt(x_new + 100) + "\" y=\"" + parseInt(y_new-10) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"0\"></text>" +
                "<rect class=\"addpartnerR\" width=\"20\" height=\"80\" x=\"" + parseInt(x_new + 200) + "\" y=\"" + parseInt(y_new + 10) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addpartnerR\" transform=\"rotate(90)\" x=\"" + parseInt(y_new + 10 + 40) + "\" y=\"" + inverse_parentR_rect_x + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"22\" data-have=\"0\"></text>" +
                "<rect class=\"addpartnerL\" width=\"20\" height=\"80\" x=\"" + parseInt(x_new - 20) + "\" y=\"" + parseInt(y_new + 10) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addpartnerL\" transform=\"rotate(90)\" x=\"" + parseInt(y_new + 10 + 40) + "\" y=\"" + inverse_parentL_rect_x + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\"text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"23\" data-have=\"0\"></text>" +
                "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"" + parseInt(x_new + 10) + "\" y=\"" + parseInt(y_new + 100) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                "<text class=\"addchildren\" x=\"" + parseInt(x_new + 100) + "\" y=\"" + parseInt(y_new + 110) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"0\"></text>" +
                "<image class=\"nodeImage\" xlink:href=\"\" x=\"" + parseInt(x_new + 130) + "\" y=\"" + parseInt(y_new + 30) +"\" height=\"60\" width=\"60\"/>" +
            "</g>";

        $(".tree").html($(".tree").html()+newNode);
        addOnClickToAllNodes();
        addImageToAllNodes();
        
        if (if_add_father)
        {
            var parentL_rect_x = parseInt(x_new_father - 20);
            var parentR_rect_x = parseInt(x_new_father + 200);

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


            var newNodeFather = // dodajemy też ojca bo matka dodana wyżej
                "<g class=\"tree-elements\" onmousedown=\"mousedowntreeelement(evt)\" transform=\"matrix(1 0 0 1 0 0)\" data-mainuser=\"0\">" +
                    "<rect class=\"tree-element-frames\" width=\"200\" height=\"100\" x=\"" + x_new_father + "\" y=\"" + y_new_father + "\" fill=\"white\" stroke=\"black\"/>" +
                    "<text class=\"tree-element-texts\" x=\"" + parseInt(x_new_father + 100) + "\" y=\"" + parseInt(y_new_father + 15) + "\" font-family=\"verdana\" font-size=\"12\" fill=\"blue\" alignment-baseline=\"middle\" text-anchor=\"middle\">Test</text>" +
                    "<rect class=\"addparents\" width=\"180\" height=\"20\" x=\"" + parseInt(x_new_father + 10) + "\" y=\"" + parseInt(y_new_father - 20) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                    "<text class=\"addparents\" x=\"" + parseInt(x_new_father + 100) + "\" y=\"" + parseInt(y_new_father - 10) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"20\" data-have=\"0\"></text>" +
                    "<rect class=\"addpartnerR\" width=\"20\" height=\"80\" x=\"" + parseInt(x_new_father + 200) + "\" y=\"" + parseInt(y_new_father + 10) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                    "<text class=\"addpartnerR\" transform=\"rotate(90)\" x=\"" + parseInt(y_new_father + 10 + 40) + "\" y=\"" + inverse_parentR_rect_x + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"22\" data-have=\"0\"></text>" +
                    "<rect class=\"addpartnerL\" width=\"20\" height=\"80\" x=\"" + parseInt(x_new_father - 20) + "\" y=\"" + parseInt(y_new_father + 10) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                    "<text class=\"addpartnerL\" transform=\"rotate(90)\" x=\"" + parseInt(y_new_father + 10 + 40) + "\" y=\"" + inverse_parentL_rect_x + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\"text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"23\" data-have=\"0\"></text>" +
                    "<rect class=\"addchildren\" width=\"180\" height=\"20\" x=\"" + parseInt(x_new_father + 10) + "\" y=\"" + parseInt(y_new_father + 100) + "\" fill=\"white\" stroke=\"red\" visibility=\"hidden\" data-have=\"0\" onclick=\"addNewNodeToTreeClick(this)\"/>" +
                    "<text class=\"addchildren\" x=\"" + parseInt(x_new_father + 100) + "\" y=\"" + parseInt(y_new_father + 110) + "\" font-family=\"verdana\" font-size=\"6\" fill=\"black\" alignment-baseline=\"middle\" text-anchor=\"middle\" visibility=\"hidden\" data-tag=\"24\" data-have=\"0\"></text>" +
                    "<image class=\"nodeImage\" xlink:href=\"\" x=\"" + parseInt(x_new_father + 130) + "\" y=\"" + parseInt(y_new_father + 30)+"\" height=\"60\" width=\"60\"/>" +
                "</g>";

                $(".tree").html($(".tree").html() + newNodeFather);
                addOnClickToAllNodes();
                addImageToAllNodes();
        }
        
        addTextsToAddButtonsAllNodes(languageID); // dodanie tekstów dla nowych węzłów
    }

    function mousedowntreeelement(evt)
    {
        g_element = $(evt.target).parent();
        main_rect = $(evt.target);

        //włączenie kursora do przesuwania
        main_rect.css('cursor', 'move');

        startx = evt.clientX;
        starty = evt.clientY;
        oldMatrix = $(g_element).attr("transform");
        oldMatrix = oldMatrix.substring(7, oldMatrix.length - 1).split(' ');
        
        for (var i = 0; i < oldMatrix.length; i++) {
            oldMatrix[i] = parseFloat(oldMatrix[i]);
        }

        $(main_rect).on('mousemove', function (evt) {
            var divider = $(".tree").attr("transform");
            divider = divider.substring(7, divider.length - 1).split(' ');

            dx = evt.clientX - startx;
            dy = evt.clientY - starty;
            oldMatrix[4] += dx / divider[0]; // podzielić na matrix svg
            oldMatrix[5] += dy / divider[0];
            
            var newMatrix = "matrix(" + oldMatrix.join(' ') + ")";
            $(g_element).attr('transform', newMatrix)

            startx = evt.clientX;
            starty = evt.clientY;
        });

        $(main_rect).on('mouseup', function () {
            if (g_element != null && main_rect != null)
            {
                main_rect.css('cursor', 'default');
                $(main_rect).off("mousemove");
                $(main_rect).off("mouseout");
                $(main_rect).off("mouseup");
                startx, starty, dx, dy, g_element, main_rect = null;
            }
        });

        $(main_rect).on('mouseout', function () {
            if (g_element != null && main_rect != null) {
                main_rect.css('cursor', 'default');
                $(main_rect).off("mousemove");
                $(main_rect).off("mouseout");
                $(main_rect).off("mouseup");
                startx, starty, dx, dy, g_element, main_rect = null;
            }
        });

        $("#svg_container").on('mouseout', function () {
            if (g_element != null && main_rect != null) {
                main_rect.css('cursor', 'default');
                $(main_rect).off("mousemove");
                $(main_rect).off("mouseout");
                $("#svg_container").off("mouseup");
                startx, starty, dx, dy, g_element, main_rect = null;
            }
        });
    }

    function addTextsToAddButtonsAllNodes(languageID)
    {
        $("text[class^=add]").each(function (index, element) {
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

    function addOnClickToNewNodes()
    {
        $('.tree-elements:last-child').on('click', function () {
            if (lastClickedRect !== null) {
                lastClickedRect.children('.tree-element-frames-active').attr('stroke', 'black');
                lastClickedRect.children('.tree-element-frames-active').attr('class', 'tree-element-frames');
                //chowanie przycisków do dodawania
                lastClickedRect.children('*[class^=add][data-have=0]').attr('visibility', 'hidden');

                if (lastClickedRect.attr('id') === $(this).attr('id')) // gdy chcemy odznaczyć zaznaczonego
                {
                    lastClickedRect = null;
                    return;
                }

                $(this).children('.tree-element-frames').attr('stroke', 'red');
                $(this).children('.tree-element-frames').attr('class', 'tree-element-frames-active');
                //wyświetlenie przycisków do dodawania członków rodziny
                $(this).children('*[class^=add][data-have=0]').attr('visibility', 'visible');

                lastClickedRect = $(this);
            }

            $(this).children('.tree-element-frames').attr('stroke', 'red');
            $(this).children('.tree-element-frames').attr('class', 'tree-element-frames-active');
            //wyświetlenie przycisków do dodawania członków rodziny
            $(this).children('*[class^=add][data-have=0]').attr('visibility', 'visible');

            lastClickedRect = $(this);
        });
    }

    function addOnClickToAllNodes() {
        $('.tree-elements').on('click', function () {
            if (lastClickedRect !== null) {
                lastClickedRect.children('.tree-element-frames-active').attr('stroke', 'black');
                lastClickedRect.children('.tree-element-frames-active').attr('class', 'tree-element-frames');
                //chowanie przycisków do dodawania
                lastClickedRect.children('*[class^=add][data-have=0]').attr('visibility', 'hidden');

                if (lastClickedRect.attr('id') === $(this).attr('id')) // gdy chcemy odznaczyć zaznaczonego
                {
                    lastClickedRect = null;
                    return;
                }

                $(this).children('.tree-element-frames').attr('stroke', 'red');
                $(this).children('.tree-element-frames').attr('class', 'tree-element-frames-active');
                //wyświetlenie przycisków do dodawania członków rodziny
                $(this).children('*[class^=add][data-have=0]').attr('visibility', 'visible');

                lastClickedRect = $(this);
            }
            else
            {
                $(this).children('.tree-element-frames').attr('stroke', 'red');
                $(this).children('.tree-element-frames').attr('class', 'tree-element-frames-active');
                //wyświetlenie przycisków do dodawania członków rodziny
                $(this).children('*[class^=add][data-have=0]').attr('visibility', 'visible');

                lastClickedRect = $(this);
            }
        });
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