# homework-2

Домашнее задание по итогам второй недели.
Необходимо разработать два сервиса.
1. Сервис-эмулятор погодных датчиков.
	Сервис должен генерировать события от минимум двух погодных датчиков (можно больше) уличного и датчика внутри помещения
	В событии от датчиков должны быть данные по текущей температуре, влажности и содержанию CO2. Желательно учесть, что уровень CO2 на уличном датчике обычно +- одинаковый, в отличии от датчиков, находящихся в помещениях.
	Должен реализовывать Grpc-сервис, который в потоковом режиме будет возвращать события из датчиков.
	Должен реализовывать REST-метод, который возвращает текущие параметры каждого датчика. Например, сейчас нужно прямо узнать какие последние данные были на датчике.
	Интервал генерации событий вынести в настройки.
	Интервал не должен превышать 2 секунд. Т.е. события должны генерироваться хотя бы одно в 2 секунды по каждому датчику.
	Не стоит выставлять и слишком маленький интервал - минимальное значение 100мс.
	
2. Сервис-клиент обработки событий от сервиса-эмулятора погодных датчиков
Должен уметь подписывается на получение данных от конкретного датчика или группы датчиков.
Должен уметь отписываться от получения информации по одному или всем датчикам.
Должен взаимодействовать с сервисом-эмулятором через полнодуплексный grpc stream.
Должен уметь переподнимать поток, если вдруг происходит разрыв связи. Например, если сервис-эмулятор остановлен, то необходимо пробовать подключаться с нему, до победного. Плюсом будет использование более сложного алгоритма ожидания, чем простой Delay.
	
Должен оперативно аггрегировать информацию в следующих разрезах:
	1. Средняя температура по каждому датчику за 1 минуту.
	2. Средняя влажность по каждому датчику за 1 минуту.
	3. Максимальное и минимальное содержание CO2 по каждому датчику за 1 минуту.
		
Должен иметь ручки, для чтения агрегированных данных в разрезе указанного интервала. Например, я хочу получить среднюю температуру за 10 минут начиная с 13:44. Значит ручка должна вытащить уже сагрегированные данные по 1 минуте, и посчитать из них среднюю за 10 минут.
Должен иметь настройку для изменения интервала аггрегации. Если нужно аггрегировать не за 1 минуту, а за 10 минут.
Должен иметь ручку для диагностики, которая выводит все сохраненные данные по каждому датчику.
	
При разработке не использовать никаких внешних источников данных (БД,редис и т.п.).
Все данные хранить в оперативной памяти. Данные должны быть доступны только на время работы сервиса.
Допускается расширить функциональность сервисов по своему усмотрению, но минимальный набор требований должен быть выполнен обязательно.
Критерии корректно выполнения задания:
	1. Все ручки реализованы
	2. Сервис-эмулятор генерирует события, а сервис-клиент эти события получает и обрабатывает
	3. На каждый запрос в ручку сервиса клиента - мы получаем корректно рассчитанный ответ.
	4. Код должен быть написан чисто, разделен на логические блоки. Желательно использовать стандартные настройки code style в райдере.	
	5. Клиент всегда может восстановить связь с сервисом-эмулятором.