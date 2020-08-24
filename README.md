# 1 Analiza kodu źródłowego
## 1.1 Co robi ten kod?
W pliku HttpRequestHandler.cs znajduje się:
Abstakcyjna generyczna klasa HttpRequestHandler z parametrami TRequest, TResponse. 
- pole _httpClientProxy z referencją obiektu klasy HttpClient
- właściwość _parser zawierące referencje do obiektu klasy implementującej interfejs IHttpResponseParser<TResponse>. 
- konstruktor przypisujący refernecje do pola _parser.
- metoda asynchroniczna zwracająca Task<TResponse> Handle(). Zadaniem tej metody jest wysłanie zapytania HTTP i zwrócenie przetworzonej odpowiedzi za pomocą _parser,
   metoda ta jako parametry otrzymuje nazwę metody HTTP, URL, body, ewentualne nagłówki oraz obiekt klasy CancellationToken odpowiedzialny za anulowanie zapytania.
   Na podstawie otrzymanych parametrów metoda tworzy obiekt klasy HttpRequestMessage i poprzez metodę klasy HttpClient.SendAsync() wysyła zapytanie pod podany URL, następnie        otrzymaną odpowiedź parsuje przy użyciu metody ParseAsync() dostępnej w obiekcie _parse. 
   
Generyczny interfejs IHttpResponseParser z parametrami TResult.
- w ramach tego interfejsu mamy metodę ParseAsync() otrzymującą obiekt klasy HttpResponseMessage, zwracającą obiekt typu TResult

## 1.2 Jakie widać problemy
- naruszenie zasady Single Responsibility Principle: metoda Handle() jest odpowiedzialna za stworzenie obiektu HttpRequestMessage na podstawie parametrów i przetworzenie tego      zapytania - można to rozdzielić.
- otrzymany w konstruktorze parametr httpClientProxy nie jest nigdzie przypisany.
- do metody Handle() wysyłany jest obiekt typu TRequest i nie jest wykorzystany
- w bloku warunkowym if(!response.IsSuccessStatusCode) rzucamy wyjątkiem EndOfStreamException, który nie jest odpowiednim typem wyjątku w takim przypadku.
- rzucanie wyjatku NotImplementedException()
- zupełnie niepotrzebne pole Variable1, zbędne nowe linie, nieodpowiednie nazwenictwo zmiennych(pleaseProvideFullUrlHere), niepotrzebne przypisania(var headers =                    additionalHeaders), nieodpowiednio skonstruowane bloki warunkowe.

## 1.3 Co jest fajnego? 
 Przede wszystkim generyczność klasy HttpRequestHandler - możemy do klasy wstrzyknąć dowolny obiekt implementujący IHttpResponseParser<TResponse> i zwrócić sparsowaną odpowiedź   do dowolnego typu. 
 W swoim rozwiązaniu stworzyłem klase HttpResponseToStringParser, która po prostu zwraca otrzymaną odpowiedź pod postacią stringa, lecz równie dobrze mogłaby być to klasa         parsująca odpowiedź do obiektów danego typu, np User.

 Nie do końca jasny był dla mnie cel parametru generycznego TRequest i co mogłoby się pod nim kryć, w metodzie Handle() wysyłamy zapytanie poprzez metodę HttpClient.SendAsync(), która przyjmuje obiekt klasy HttpRequestMessage, 
 a ta zawiera wszystkie potrzebne informacje potrzebne do wykonania zapytania. Wykorzystałem więc fakt, że "wszystkie chwyty dozwolone" i usunąłem go.

## 1.4 Jakie widzimy niebezpieczeństwa używając tej metody?
 Przede wszystkim rzuciło mi się w oczy to, że wykorzystujemy obiekt klasy HttpClient per obiekt klasy HttpRequestHandler. Niesie to za sobą ryzyko, że w przypadku stworzenia dużej ilości takich obiektów możemy doprowadzić do 
 wyczerpania wszystkich dostępnych socketów i spadku performance'u. Rozwiązaniem tego problemu jest uczynienie obiektu HttpClient statycznym - jedna instancja będzie wykorzystywana przez wszystkie obiekty.   
 Druga sprawa to że wywołujemy metodę HttpClient.SendAsync(HttpRequestMessage, CancellationToken), która ładuje całą odpowiedź do pamięci. W przypadku, gdy pobieramy duży fragment danych powinniśmy wywołać metodę 
 HttpClient.SendAsync(HttpRequestMessage, HttpCompletionOption, CancellationToken). Domyślną wartością parametru HttpCompletionOption w pierwszym przypadku jest HttpCompletionOption.ResponseContentRead - operacja jest uznana 
 za zakończoną po odczytaniu całej odpowiedzi. Natomiast ustawiając ten parametr na wartość HttpCompletionOption.ResponseHeadersRead umożliwia na rozpoczęcie odczytywania odpowiedzi zaraz po odczytaniu nagłówków wykorzystując 
 strumień.

# 2. Refactoring, Unit test, Demo
 Jak już wspomniałem wcześniej usunąłem z klasy HttpRequestHandler parametr generyczny TRequest. Zamiast tego metoda Handle zamiast sztywnego obiektu HttpRequestMessage otrzymuje obiekt implementujący interfejs IRequest.
 W swoim rozwiązaniu stworzyłem klase HttpRequest, która go implementuje - jest swego rodzaju "wrapperem" dla HttpRequestMessage, odpowiedzialnym za stworzenie i przechowywanie obiektu tej klasy. 

 Po abstrakcyjnej klasie HttpRequestHandler dziedziczy klasa DefaultHttpRequestHandler z metodą ProccessRequest(), która otrzymuje parametry zapytania HTTP, tworzy obiekt HttpRequest i przesyła do do metody Handle(). Metoda posiada
 również parametr allowedDelay na podstawie którego tworzy obiekt CancellationToken, który umożliwia anulowanie zapytania po płynięciu żądanego czasu w milisekundach.

 W metodzie Handle() wyłapuje wyjątek typu FormatException - załozyłem, że metoda ParseAsync(), może go rzucać w przypadku gdy nie da się sparsować odpowiedzi do danego typu.

 Do testów i w aplikacji konsolowej użyłem ogólnodostępnych, darmowych fake'owych API. W 4 testach waliduje najbardziej "typowe" przypadki (wszystkie zielone o ile server nie strzeli 500 :)). W aplikacji konsolowej nie tworzyłem 
 żadnego skomplikowanego interfejsu z użytkownikiem, a jedynie wykonuje 3 przykładowe zapytania wraz z opisem - mam nadzieje, że to wystarczy.

# 3. Podsumowanie
 Zadanie bardzo ciekawe, do tej pory zadania rekrutacyjne z którymi się spotkałem polegały na stworzeniu czegoś "od zera", w tym przypadku również dochodzi zrozumienie "co autor miał na myśli" tworząc kod co w przypadku
 programisty ma miejsce bardzo często. W razie jakichkolwiek pytań co do rozwiązania, feedback'u lub ewentualnych improvementów proszę śmiało pytać :)
