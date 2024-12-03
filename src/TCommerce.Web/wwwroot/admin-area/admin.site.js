
// Lấy tất cả các phần tử input trên trang
const inputs = document.getElementsByTagName('input');

// Lặp qua từng phần tử input và áp dụng kiểm tra giá trị tối đa
for (let i = 0; i < inputs.length; i++) {
    const input = inputs[i];

    // Kiểm tra chỉ khi input có thuộc tính "max" và là kiểu number
    if (input.max && input.type === 'number') {
        input.addEventListener('input', function () {
            if (parseInt(input.value) > parseInt(input.max)) {
                input.value = input.max;
            }
        });
    }
}

function addParameterFunc(url, parameterName, parameterValue, atStart/*Add param before others*/) {
    replaceDuplicates = true;
    if (url.indexOf('#') > 0) {
        var cl = url.indexOf('#');
        urlhash = url.substring(url.indexOf('#'), url.length);
    } else {
        urlhash = '';
        cl = url.length;
    }
    sourceUrl = url.substring(0, cl);

    var urlParts = sourceUrl.split("?");
    var newQueryString = "";

    if (urlParts.length > 1) {
        var parameters = urlParts[1].split("&");
        for (var i = 0; (i < parameters.length); i++) {
            var parameterParts = parameters[i].split("=");
            if (!(replaceDuplicates && parameterParts[0] == parameterName)) {
                if (newQueryString == "")
                    newQueryString = "?";
                else
                    newQueryString += "&";
                newQueryString += parameterParts[0] + "=" + (parameterParts[1] ? parameterParts[1] : '');
            }
        }
    }
    if (newQueryString == "")
        newQueryString = "?";

    if (atStart) {
        newQueryString = '?' + parameterName + "=" + parameterValue + (newQueryString.length > 1 ? '&' + newQueryString.substring(1) : '');
    } else {
        if (newQueryString !== "" && newQueryString != '?')
            newQueryString += "&";
        newQueryString += parameterName + "=" + (parameterValue ? parameterValue : '');
    }
    return urlParts[0] + newQueryString + urlhash;
};

function UrlBuilder(baseUrl) {
    this.baseUrl = baseUrl;
    this.params = {};

    this.addParameter = function (key, value) {
        this.params[key] = value;
    };

    this.build = function () {
        var url = this.baseUrl;
        var isFirstParam = true;

        if (url.indexOf('?') === -1) {
            isFirstParam = true;
        } else {
            isFirstParam = false;
        }

        for (var key in this.params) {
            if (this.params.hasOwnProperty(key)) {
                var paramValue = this.params[key];
                if (isFirstParam) {
                    url = addParameterFunc(url, key, paramValue, true); // Thêm tham số vào đầu URL
                    isFirstParam = false;
                } else {
                    url = addParameterFunc(url, key, paramValue, false); // Thêm tham số vào cuối URL
                }
            }
        }
        return url;
    };
}
function toggleElement(checkboxId, elementId) {
    if ($(checkboxId).is(":checked")) {
        $(elementId).css('display', 'block');
    } else {
        $(elementId).css('display', 'none');
    }
}
function openPopup(url) {
    var screenWidth = window.screen.availWidth;
    var screenHeight = window.screen.availHeight;

    var popupWidth = 600;
    var popupHeight = 90 * screenHeight / 100;

    var leftPosition = (screenWidth - popupWidth) / 2;
    var topPosition = 0;

    var popupFeatures = 'width=' + popupWidth + ',height=' + popupHeight + ',left=' + leftPosition + ',top=' + topPosition + ',scrollbars=yes';

    var popupWindow = window.open(url, 'Popup', popupFeatures);
    popupWindow.focus();
    return false;
}
var entityMap = {
    '&': '&amp;',
    '<': '&lt;',
    '>': '&gt;',
    '"': '&quot;',
    "'": '&#39;',
    '/': '&#x2F;',
    '`': '&#x60;',
    '=': '&#x3D;'
}, selectedIds = [];

function escapeHtml(string) {
    return String(string).replace(/[&<>"'`=\/]/g, function (s) {
        return entityMap[s];
    });
}

function addAntiForgeryToken(n) {
    n || (n = {});
    var t = $("input[name=__RequestVerificationToken]");
    return t.length && (n.__RequestVerificationToken = t.val()),
        n
}

function updateTable(n, t) {
    $(n).DataTable().ajax.reload();
    $(n).DataTable().columns.adjust();
    t && clearSelectAllCheckbox(n)
}

function clearSelectAllCheckbox(n) {
    $(n).prop('checked', false);
}

