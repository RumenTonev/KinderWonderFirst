﻿var jcrop_api,
      boundx,
      boundy,
      xsize,
      ysize;

// ToDo - change the size limit of the file. You may need to change web.config if larger files are necessary.
var maxSizeAllowed = 2;     // Upload limit in MB
var maxSizeInBytes = maxSizeAllowed * 1024 * 1024;
var keepUploadBox = false;  // ToDo - Remove if you want to keep the upload box
var keepCropBox = false;    // ToDo - Remove if you want to keep the crop box

$(function () {
    if (typeof $('#avatar-upload-form') !== undefined) {
        initAvatarUpload();
        $('#avatar-max-size').html(maxSizeAllowed);
        $('#avatar-upload-form input:file').on("change", function (e) {
            debugger;
            var files = e.currentTarget.files;
            for (var x in files) {
                if (files[x].name != "item" && typeof files[x].name != "undefined") {
                    if (files[x].size <= maxSizeInBytes) {
                        // Submit the selected file
                        $('#avatar-upload-form .upload-file-notice').removeClass('bg-danger');
                        $('#avatar-upload-form').submit();
                    } else {
                        // File too large
                       
                        $("#avatar-message").toggle();
                            debugger;
                            setTimeout(function () {
                                $("#avatar-message").toggle();
                            }, 10000);
                            
                        
                        $('#avatar-upload-form .upload-file-notice').addClass('bg-danger');
                    }
                }
            }
        });
    }
});

function initAvatarUpload() {
    $('#avatar-upload-form').ajaxForm({
        beforeSend: function () {
            $('#avatar-upload-form').addClass('hidden');
        },
        uploadProgress: function (event, position, total, percentComplete) {
            updateProgress(percentComplete);
        },
        success: function (data) {
            if (data.success === false) {
                $('#avatar-upload-form').removeClass('hidden');
                toastr.error(data.errorMessage);
            } else {
                $("#approve-btn").removeAttr("disabled");
                $('#preview-pane .preview-container img').attr('src', data.fileName);

                var img = $('#crop-avatar-target');
                img.attr('src', data.fileName);

                if (!keepUploadBox) {
                    $('#avatar-upload-box').addClass('hidden');
                }
                $('#avatar-crop-box').removeClass('hidden');
                initAvatarCrop(img);
            }
        },
        complete: function (xhr) {
        }
    });
}

function updateProgress(percentComplete) {
    $('.upload-percent-bar').width(percentComplete + '%');
    $('.upload-percent-value').html(percentComplete + '%');
    if (percentComplete === 0) {
        $('#upload-status').empty();
        $('.upload-progress').removeClass('hidden');
    }
}

function initAvatarCrop(img) {
    img.Jcrop({
        onChange: updatePreviewPane,
        onSelect: updatePreviewPane,
        aspectRatio: xsize / ysize
    }, function () {
        var bounds = this.getBounds();
        boundx = bounds[0];
        boundy = bounds[1];

        jcrop_api = this;
        jcrop_api.setOptions({ allowSelect: true });
        jcrop_api.setOptions({ allowMove: true });
        jcrop_api.setOptions({ allowResize: true });
        jcrop_api.setOptions({ aspectRatio: 1 });

        // Maximise initial selection around the centre of the image,
        // but leave enough space so that the boundaries are easily identified.
        var padding = 10;
        var shortEdge = (boundx < boundy ? boundx : boundy) - padding;
        var longEdge = boundx < boundy ? boundy : boundx;
        var xCoord = longEdge / 2 - shortEdge / 2;
        jcrop_api.animateTo([xCoord, padding, shortEdge, shortEdge]);

        var pcnt = $('#preview-pane .preview-container');
        xsize = pcnt.width();
        ysize = pcnt.height();
        $('#preview-pane').appendTo(jcrop_api.ui.holder);
        jcrop_api.focus();
    });
}

function updatePreviewPane(c) {
    debugger;
    if (parseInt(c.w) > 0) {
        var rx = xsize / c.w;
        var ry = ysize / c.h;

        $('#preview-pane .preview-container img').css({
            width: Math.round(rx * boundx) + 'px',
            height: Math.round(ry * boundy) + 'px',
            marginLeft: '-' + Math.round(rx * c.x) + 'px',
            marginTop: '-' + Math.round(ry * c.y) + 'px'
        });
        $('#picWidth').val(c.w);
        $('#picHeight').val(c.h);
        $('#picTop').val(c.y);
        $('#picLeft').val(c.x);
        $('#picBottom').val($('#preview-pane .preview-container img').css('marginBottom'));
        $('#picRight').val($('#preview-pane .preview-container img').css('marginRight'));
        $('#picName').val($('#preview-pane .preview-container img').attr("src"));
    }
}

