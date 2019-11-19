# badAttemptProducerCustomerTask
Task: "Синхронизация процессов и потоков

Написать программу, реализующую модель "поставщик-потребитель" с буфером длины 10. Участвующие в модели процессы реализовать в виде нитей. Предусмотреть независимое управление работой каждой нити: запуск, остановку, паузу в работе и изменение скорости работы. Для наглядности содержимое буфера должно отображаться в интерфейсе программы.

Воспользоваться стандартным классом Thread, где реализованы все необходимые методы."

![alt text](https://user-images.githubusercontent.com/46371199/69119910-c47d6080-0ab9-11ea-801b-6be4cbc76b30.jpg)
Bad bug: now everything works, but if I remove the delay in the mutex and create a lot of threads with high speed, the program crashes, maybe it can’t cope with the speed and number of threads, or maybe the implementation bug
