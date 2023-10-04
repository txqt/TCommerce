
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

