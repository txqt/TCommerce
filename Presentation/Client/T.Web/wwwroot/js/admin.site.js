
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

