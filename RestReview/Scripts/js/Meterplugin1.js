(function ($) {
    var global_settings = {};
    //For Default Circular Progress Bar
    var DefaultCircularBar_methods = {
        init: function (options) {
            var settings = $.extend({
                color: "#A68080",
                height: "300px",
                width: "300px",
                line_width: 8,
                starting_position: 25,
                percent: 100,
                counter_clockwise: false,
                percentage: true,
                text: ''
            }, options);
            global_settings = settings;

            var percentage = $("<div class='progress-percentage'></div>");
            if (!global_settings.percentage) {
            }
            $(this).append(percentage);
            var text = $("<div class='progress-text'></div>");
            if (global_settings.text != "percent") {
            }
            $(this).append(text);

            if (global_settings.starting_position != 100) {
                global_settings.starting_position = global_settings.starting_position % 100;
            }
            if (global_settings.ending_position != 100) {
                global_settings.ending_position = global_settings.ending_position % 100;
            }
            appendUnit(global_settings.width);
            appendUnit(global_settings.height);

            $(this).css({
                "height": global_settings.height,
                "width": global_settings.width
            });
            $(this).addClass("circular-progress-bar");
            $(this).find("canvas").remove();
            $(this).append(createCanvas($(this)));
            return this;
        },

        percent: function (value) {
            global_settings.percent = value;
            $(this).css({
                "height": global_settings.height,
                "width": global_settings.width
            });
            $(this).children("canvas").remove();
            $(this).append(createCanvas($(this)));
            return this;
        },

        defaultanimate: function (value, time) {
            debugger
            $(this).css({
                "height": global_settings.height,
                "width": global_settings.width
            });
            var num_of_steps = time / 10;
            var percent_change = (value - global_settings.percent) / num_of_steps;
            var scope = $(this);
            var theInterval = setInterval(function () {
                if (global_settings.percent < value) {
                    scope.children("canvas").remove();
                    global_settings.percent += percent_change;
                    scope.append(createCanvas(scope));
                } else {
                    clearInterval(theInterval);
                }
            }, 10);
            return this;
        }
    };

    $.fn.defaultcircularProgress = function (methodOrOptions) {
        if (DefaultCircularBar_methods[methodOrOptions]) {
            return DefaultCircularBar_methods[methodOrOptions].apply(this, Array.prototype.slice.call(arguments, 1));
        } else if (typeof methodOrOptions === 'object' || !methodOrOptions) {
            return DefaultCircularBar_methods.init.apply(this, arguments);
        } else {
            $.error('Method ' + methodOrOptions + ' does not exist.');
        }
    };

    //End

    /* =========================================================================
        PRIVATE FUNCTIONS
    ========================================================================= */

    // return string without 'px' or '%'
    function removeUnit(apples) {
        if (apples.indexOf("px")) {
            return apples.substring(0, apples.length - 2);
        } else if (canvas_height.indexOf("%")) {
            return apples.substring(0, apples.length - 1);
        }
    };
    // return string with 'px'
    function appendUnit(apples) {
        if (apples.toString().indexOf("px") < -1 && apples.toString().indexOf("%") < -1) {
            return apples += "px";
        }
    };
    // calculate starting position on canvas
    function calcPos(apples, percent) {
        if (percent < 0) {
            // Calculate starting position
            var starting_degree = (parseInt(apples) / 100) * 360;
            var starting_radian = starting_degree * (Math.PI / 180);
            return starting_radian - (Math.PI / 2);
        } else {
            // Calculate ending position
            var ending_degree = ((parseInt(apples) + parseInt(percent)) / 100) * 360;
            var ending_radian = ending_degree * (Math.PI / 180);
            return ending_radian - (Math.PI / 2);
        }
    };
    // Put percentage or custom text inside progress circle
    //this is working checked by dipa
    function insertText(scope) {
        //$(".progress-percentage").text(Math.round(global_settings.percent) + "%");
        //  $(".progress-percentage").text((global_settings.percent / 10).toFixed(1));
    }
    // create canvas
    function createCanvas(scope) {
        // Remove 'px' or '%'
        var canvas_height = removeUnit(global_settings.height.toString());
        var canvas_width = removeUnit(global_settings.width.toString());

        // Create canvas
        var canvas = document.createElement("canvas");
        //canvas.height = canvas_height;
        canvas.height = 180;
        canvas.width = canvas_width;

        // Create drawable canvas and apply properties
        var ctx = canvas.getContext("2d");
        ctx.strokeStyle = global_settings.color;
        ctx.lineWidth = global_settings.line_width;

        // Draw arc
        ctx.beginPath();

        // Calculate starting and ending positions
        var starting_radian = calcPos(global_settings.starting_position, -1);
        var ending_radian = calcPos(global_settings.starting_position, global_settings.percent);
        // Calculate radius and x,y coordinates
        var radius = 0;
        var xcoord = canvas_width / 2;
        var ycoord = canvas_height / 2;
        // Height or width greater
        if (canvas_height >= canvas_width) {
            radius = canvas_width * 0.9 / 2 - (global_settings.line_width * 2);
        } else {
            radius = canvas_height * 0.9 / 2 - (global_settings.line_width * 2);
        }

        /*
            x coordinate
            y coordinate
            radius of circle
            starting angle in radians
            ending angle in radians
            clockwise (false, default) or counter-clockwise (true)
        */
        ctx.arc(xcoord, ycoord, radius, starting_radian, ending_radian, global_settings.counter_clockwise);
        ctx.stroke();

        // Add text
        if (global_settings.percentage) {
            //     insertText(scope);
        }
        return canvas;
    };
}(jQuery));