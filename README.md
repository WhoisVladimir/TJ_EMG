# TJ_EMG
___

![](https://i.postimg.cc/26XbmkRS/112.png)
![](https://i.postimg.cc/7YBPfxK3/113.png) ![](https://i.postimg.cc/prpGCJtX/114.png)
![](https://i.postimg.cc/0273BZwb/115.png) ![](https://i.postimg.cc/QMdyV3nT/116.png)
![](https://i.postimg.cc/N0FdBx58/117.png) ![](https://i.postimg.cc/D0k6Qk55/118.png)

Проект - результат тестового задания, полученного от компании "Единая Медиа Группа".
В проекте используются технологии: камера Cinemachine, управление New Input System, Shader Graph. Реализовано: перемещание камеры по карте, приближение камеры по плавной траектории к наблюдаемой точке с изменением угла обзора, последующее возвращение камеры в исходное положение, карта высот поверхности (гипсометрическая шкала) с использованием Shader Graph, взаимодействие с UI в  соответствии с поставленными задачами. 

### Текст задания:
Вам необходимо сделать демо версию программы для быстрой расстановки и редактирования плашек с текстом.  Заказчик очень любит расставлять плашки на неровной земной поверхности и очень надеется, что у вас получится создать софт, который позволит это сделать максимально удобно и приятно. 
Пояснительный рисунок:
![](https://i.postimg.cc/FF2J1HqM/image.jpg)
Вот какие требования у клиента:
При запуске программы мы видим 3D пространство с неровной земной поверхностью (подложкой) и простым интерфейсом.
Подложка в зависимости от высоты окрашена в разные цвета по произвольной гипсометрической шкале.
Для перемещения и вращения камеры используется правая кнопка мыши (ПКМ). Нажатие и удерживание ПКМ - перемещение «хватанием» за пол (как в google картах). Shift+ПКМ - вращение камеры вокруг точки, на которую смотрит камера. Ни при каких условиях камера не должна уходить под землю.
При нажатии на кнопку "создать плашку" в интерфейсе мы переходим в режим создания плашек. Если в этом режиме кликнуть левой кнопкой мыши (ЛКМ) по полу, то над ним создается плашка, после чего режим создания отключается и сразу открывается интерфейс редактирования текста новой плашки.  Появление созданной плашки должно быть с анимацией (придумайте сами).
При нажатии ЛКМ на созданную плашку, она выделяется и появляется окно интерфейса, в котором можно изменить текст у выделенной плашки. При изменении текста, размеры плашки подстраиваются под размеры текста.  Появление и исчезновение интерфейса редактирования должно быть анимировано (придумайте сами как).
Если мы НЕ в режиме "создать плашку", то клик ЛКМ в пустоту/пол снимает выделение с плашек.
Выделив плашку и удерживая ЛКМ, плашку можно перетаскивать. Если удерживать ALT меняется высота расположения плашки над землей, иначе плашка перемещается параллельно плоскости пола. Плашка никогда не должна опускаться ниже уровня поверхности земли или поднимается выше некого заданного уровня делающего плашку никогда не видимой для камеры.
Также необходимо реализовать возможность выделять и снимать выделение с нескольких плашек с использованием клавиши CTRL, с возможностью группового редактирования/удаления/перемещения выделенных плашек.
При нажатии кнопки Delete  выделенные плашки удаляются.


Оцениваются:
- Визуальное исполнение (постарайтесь сделать красиво);
- Удобство работы с программой. Ваша задача наиболее оптимальными средствами сделать удобную в работе программу с интуитивно понятным интерфейсом, ориентируясь на пожелания заказчика;
- Скорость разработки;
- Код.