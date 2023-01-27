import { useRef, useState, useEffect } from "react";
import "./App.css";

function App() {
  const subjectInputRef = useRef<any>(null);
  const publishInputRef = useRef<any>(null);

  // const [subject, setSubject] = useState("empty");
  // const [payload, setPayload] = useState("empty");
  // const [count, setCount] = useState(0);

  function postNewSubject() {
    fetch("/NewSubject", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Subject:
          subjectInputRef.current != null
            ? subjectInputRef.current.value
            : "error",
      }),
    });
  }

  function postNewMessage() {
    fetch("/PublishMessage", {
      method: "POST",
      headers: {
        Accept: "application/json",
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Payload:
          publishInputRef.current != null
            ? publishInputRef.current.value
            : "empty",
      }),
    });
  }

  // async function loadData() {
  //   try {
  //     fetch("https://localhost:7116/LastMessage")
  //       .then((res) => res.json())
  //       .then((data) => {
  //         setSubject(data.messageSubject);
  //         setPayload(data.messagePayload);
  //       });

  //     fetch("https://localhost:7116/MessageCount")
  //       .then((res) => res.text())
  //       .then((text) => setCount(+text));
  //   } catch (e) {
  //     console.log(e);
  //   }
  // }

  // useEffect(() => {
  //   loadData();
  //   setInterval(loadData, 10000);
  // });

  const [allMessages, setAllMessages] = useState<any[]>([]);

  function getAllMessages() {
    fetch("/LastMessages") // "http://localhost:3000/message1"
      .then((res) => res.json())
      .then((data) => {
        setAllMessages(data);
      });
  }

  const initialButtonText: string = "Get all Messages";
  const [buttonText, setButtonText] = useState(initialButtonText);
  const [intervalState, setIntervalState] = useState(-1);

  function manageAllMessagesInterval() {
    if (intervalState == -1) {
      setIntervalState(setInterval(getAllMessages, 1000));
      setButtonText("Stop");
    } else {
      clearInterval(intervalState);
      setIntervalState(-1);
      setButtonText(initialButtonText);
    }
  }

  return (
    <div className="App">
      <div className="card">
        <h3>
          Use "<b>&gt;</b>" to subscribe to all subjects:
        </h3>
        <input
          ref={subjectInputRef}
          type="text"
          placeholder="Message Subject"
        />
        <button onClick={postNewSubject}>Change Message Subject</button>
        <br />
        <br />
        <h3>Publish a message onto the NATS queue:</h3>
        <input ref={publishInputRef} type="text" placeholder="Payload" />
        <button onClick={postNewMessage}>Publish Message</button>
      </div>
      <div className="card">
        <button onClick={manageAllMessagesInterval}>{buttonText}</button>
        <hr />
        {allMessages.length != 0 &&
          allMessages.map((item, index) => {
            return (
              <div key={index}>
                <h2>
                  #{index} Message Subject: <code>{item.messageSubject}</code>
                </h2>
                <h4>
                  Timestamp: <code>{item.messageTimestamp}</code>
                </h4>
                <p>
                  Acknowledgement: <code>{item.messageAck}</code>
                </p>
                <p>
                  Payload:{" "}
                  <code>
                    {typeof item.messagePayload == "string"
                      ? item.messagePayload
                      : JSON.stringify(item.messagePayload)}
                  </code>
                </p>
                <hr />
              </div>
            );
          })}
      </div>
    </div>
  );
}

export default App;
