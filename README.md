# Azure & Pizza - Workshop (Bot Framework + Cognitive Services)

Workshop para el evento Azure &amp; Pizza llevado a cabo en Montevideo y organizado por Microsoft y Green Blue Technologies

## Objetivo

El objetivo de este workshop es mostrar algunas de las capacidades existentes dentro de Azure como plataforma para construir aplicaciones. Particularmente, quien siga este workshop será introducido en el mundo de los Bots/Chatbots y en cómo estos pueden ser inteligentes a partir del uso de diferentes APIs brindadas por Bing & Microsoft Cognitive Services. Estas, permiten que el desarrollador común pueda acceder a un conjunto de algoritmos trabajados y perfeccionados por expertos de Microsoft, permitiendo realzar nuestras apps y  agregar valor en una enormidad de contextos de aplicación.

## Slides de la presentación

Aquí se adjuntan las slides dadas en el evento que funcionan como introducción y marco teórico de los conceptos a poner en práctica:

- SLIDE COGNITIVE
- SLIDE BOT FR

## Pre-Workshop

### ¿Qué vamos a desarrollar? 

En este workshop construiremos un bot que está pensado para personas no videntes; la idea es que los mismos puedan tener la posibilidad de interactuar conversacionalmente con su entorno de forma conversacional. Para simplificar el escenario, el mismo se realizará sobre un chat (lo ideal sería que fuera por voz), y en un escenario de domótica.

### ¿Qué tecnologías vemos en este workshop

- Microsoft Bot Framework
- Cognitive Services: LUIS (entendimiento de lenguaje natural) 
- Cognitive Services: Computer Vision API

### Requisitos del workshop

- Microsoft Visual Studio 2015 (Enterprise o Communty) y .Net Framework 4.6 
- Bot Visual Studio Template - C# [(Descarga aquí)](http://aka.ms/bf-bc-vstemplate)
- Bot Framework Emulator (Mac/Windows) - [(Descarga aquí)](https://emulator.botframework.com/)

## Workshop - Parte 1: Ambiente de desarrollo y fundamentos básicos un bot

1) Agregamos el template de 'Bot Application' a nuestros Templates en Visual Studio. Para ello debemos descrimir el .zip y copiar la carpeta en ```C:\...\Documents\Visual Studio 2015\Templates\ProjectTemplates\Visual C#```

![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/1.png)

2) Abrimos el Visual Studio y creamos una nueva 'Bot Application', con el nombre y ubicación de destino que queramos.


![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/2.png)

3) Observar y entender el template y la estructura del proyecto creado. 


![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/3.png)

¿A qué nos recuerda?


Examinar el siguiente fragmento de código:

```C#
 public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
 {
    if (activity.Type == ActivityTypes.Message)
    {
        ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
        // calculate something for us to return
        int length = (activity.Text ?? string.Empty).Length;

        // return our reply to the user
        Activity reply = activity.CreateReply($"You sent {activity.Text} which was {length} characters");
        await connector.Conversations.ReplyToActivityAsync(reply);
    }
    else
    {
        HandleSystemMessage(activity);
    }
    var response = Request.CreateResponse(HttpStatusCode.OK);
    return response;
}
```

4) Compilar y ejecutar el proyecto, donde se abrirá una ventana del explorador indicando el puerto en el que nuestro bot está
atendiendo solicitudes

![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/4.png)

5) Abrimos el Bot Framework Emulator que deberíamos haber descargado previamente, y colocamos la ```Bot url```; es decir el endpoint en el que nuestro bot va a estar esperando recibir los mensajes por parte de los clientes.

Si tu aplicación está corriendo sobre el puerto 12345, entonces tu ```bot url``` quedaría algo así:  ```http://localhost:12345/api/messages```


![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/5.jpg)

6) Probamos nuestro bot a través del emulador y vemos lo qué sucede. ¿Qué hay detrás de cada mensaje que enviamos y recibimos mediante el bot?

![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/6.png)

## Workshop - Parte 2: Reconocimiento de Lenguaje Natural

Para esta parte utilizaremos la herramienta LUIS para que nuestro bot pueda entender las intenciones detrás de los mensajes que enviamos. De esa forma, podemos asignarle un cierto comportamiento o lógica a cada intención.

1) Lo primero que haremos será registrar nuestro modelo de LUIS en el portal brindado por Azure para acceder a dicha funcionalidad. Para ello debemos ingresar a [(luis.ai)](https://www.luis.ai/home/index/) e iniciar sesión con una cuenta de Microsoft (Ay elegir ```Import App```. De esa forma elegimos el archivo .json que existe en este repositorio y desde el cual importaremos este modelo.

![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/7.png)

Lo importante a ver aquí es que este modelo fue entrenado por quienes dictan el workshop para ahorrar el tiempo de entrenamiento del mismo. De todas formas, el modelo deberá ser mejorado y reentrenado para resolver ciertos casos que veremos en el workshop.


2) La idea ahora es entender cómo funciona este modelo, cómo se lo entrena, cómo se definen entidades e intenciones.


![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/8.png)

3) Ahora publicamos nuestro modelo yendo a la opción ```Publish App```, para ahí elegir la Comprar de crear una Key de Cognitive Services en Azure ```Build Key```. 

![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/9.png)

Para ello nos loggeamos en nuestra cuenta de Azure y agregamos un nuevo recurso del tipo 'Cognitive Services API'. Lo configuramos para que sea del tipo ```Language Understanding Intelligence Service (LUIS)```.

![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/10.png)

Finalmente nos quedamos con su Key que el valor que vamos a usar en LUIS.


![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/11.png)


Volvemos a LUIS y agregamos los datos de la Key generada, pudiendo ahora sí publicar nuestra aplicación 


![alt tag](https://github.com/piffarettig/azure-and-pizza-workshop/blob/develop/Workshop-images/12.png)


4) Ahora agregaremos la lógica creada en LUIS a nuestro bot. Para ello crearemos una nueva clase llamada RootDialog que será quien modelará la interacción con el usuario. El código de la misma se deja en las líneas siguientes. Nota: es importante decorar el nombre de la clase con el ```modelID``` y el ```subscriptionID``` correspondientes a su modelo de LUIS.

```C#

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AzureAndPizzaWorkshop
{
    [LuisModel("aa0a4611-115d-488e-a0af-36288e550c35", "d374875c80fe40ff8e7d8a085caeca64")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string intentNotFoundMessage = $"Disculpa, no pude entender tu mensaje.";
            await context.PostAsync(intentNotFoundMessage);
            context.Wait(MessageReceived);
        }

        [LuisIntent("turn_lights_on")]
        public async Task TurnLightsOn(IDialogContext context, LuisResult result)
        {
            string message = $"luces prendidas!";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

        [LuisIntent("set_alarm")]
        public async Task SetAlarm(IDialogContext context, LuisResult result)
        {
            string message = $"alarma asignada.";
            await context.PostAsync(message);
            context.Wait(MessageReceived);

        }

        [LuisIntent("turn_thermostat")]
        public async Task TurnThermostat(IDialogContext context, LuisResult result)
        {
            string message = $"accion termostato";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }

    }
}

```


Y finalmente en nuestro MessagesController, en el método POST, sustituimos el código por el siguiente:

```C#

 if (activity.Type == ActivityTypes.Message)
 {
     try
     {
         await Conversation.SendAsync(activity, () => new RootDialog());
     } catch (Exception e)
     {
         ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
         Activity reply = activity.CreateReply($"Error ocurrido: {e.Message}");
         await connector.Conversations.ReplyToActivityAsync(reply);
     }
 }
 else
 {
     HandleSystemMessage(activity);
 }
```

## Workshop - Parte 3: Agregando lógica al reconocimiento del lenguaje

En primer lugar creamos una nueva clase en el proyecto que contenga el comportamiento de desarmar las entidades e intenciones reconocidas en LUIS. La llamaremos ```LuisUtils```.

```C#
 public static class LuisUtils
 {

      public static string Entity_Time = "Time";
      public static string Entity_Sound = "Sound";
      public static string Entity_Temperature = "Temperature";
      public static string Entity_Thermostat_off = "ThermostatOff";

      private static bool TryGetEntityValueFromLuisResult(LuisResult result, out string entityDestination, string entityType)
      {
          entityDestination = string.Empty;
          EntityRecommendation title;
          if (result.TryFindEntity(entityType, out title))
          {
              entityDestination = title.Entity;
              return true;
          }
          return false;
      }

      public static bool TryGetSound(LuisResult result, out string sound)
      {
          return TryGetEntityValueFromLuisResult(result, out sound, Entity_Sound);
      }

      public static bool TryGetTime(LuisResult result, out string time)
      {
          return TryGetEntityValueFromLuisResult(result, out time, Entity_Time);
      }

      public static bool TryGetThermostatOff(LuisResult result, out string off)
      {
          return TryGetEntityValueFromLuisResult(result, out off, Entity_Thermostat_off);
      }

      public static bool TryGetTemperature(LuisResult result, out string temperature)
      {
          return TryGetEntityValueFromLuisResult(result, out temperature, Entity_Temperature);
      }

 }
```

Y ahora agregamos lógica simple sobre el manejo de los datos dentro de las resultados obtenidos con luis: las entidades e intenciones.

```C#
 [LuisIntent("set_alarm")]
 public async Task SetAlarm(IDialogContext context, LuisResult result)
 {
     string message = "";
     string time;
     string sound;
     bool soundWasFound = LuisUtils.TryGetSound(result, out sound);
     bool timeWasFound = LuisUtils.TryGetTime(result, out time);

     if (timeWasFound)
     {
         if (soundWasFound)
         {
             message = $"Alarma para las {time} activada con sonido: {sound}";
         } else
         {
             message = $"Alarma para las {time} activada, no se ha asignado sonido.";
         }
     } else
     {
         message = $"Debes asignar una hora para colocar la alarma!";
     }

     await context.PostAsync(message);
     context.Wait(MessageReceived);

 }
 ```
 
 ```C#
  [LuisIntent("turn_thermostat")]
  public async Task TurnThermostat(IDialogContext context, LuisResult result)
  {
      string message = "";
      string temperature;
      string offValue;
      bool offOrderWasFound = LuisUtils.TryGetThermostatOff(result, out offValue);
      bool temperatureWasFound = LuisUtils.TryGetTemperature(result, out temperature);

      if (offOrderWasFound)
      {
          message = "Apagando el termostato...";
      } else
      {
          if (temperatureWasFound)
          {
              message = $"Termostato activo en {temperature} grados.";
          }
          else
          {
              message = $"Debes decirme un valor de temperatura!";
          }
      }

      await context.PostAsync(message);
      context.Wait(MessageReceived);
  }
 ```
