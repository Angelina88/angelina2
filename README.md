# Задание:
	Реализовать структуру данных, представляющую собой кольцевой буфер. 
	Данные добавляются в буфер и читаются наборами страниц. Длина страницы - 8 байт.
	Непрочитанные страницы не должны затираться поступающими.
	Буфер должен потокобезопасно функционировать в условиях многопоточной (параллельной) записи и однопоточного чтения.
	
----------------------------------------------------------------------------------------------------------

Принято следующее ограничение: размер буфера должен быть кратен размеру элемента.