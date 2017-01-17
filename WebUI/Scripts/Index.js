$(function() {
    $('#singupbtn').click(function(e) {
        e.preventDefault();
        var url = '@Url.Action("Index")';
        $.post(url, $('#form').serialize(), function(data) {
            alert(data.Message);
            if (data.IsOk) {
                location.reload();
                return;
            }
            $('#@captcha.BuildInfo.ImageElementId').attr('src', data.Captcha.@captcha.BuildInfo.ImageElementId);
            $('#@captcha.BuildInfo.TokenElementId').val(data.Captcha.@captcha.BuildInfo.TokenElementId);
            $('#@captcha.BuildInfo.InputElementId').val('');
        });
    });
});