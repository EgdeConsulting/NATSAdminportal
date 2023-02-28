import { Button } from "@chakra-ui/react";

function MsgModal(props: { content: any }) {
  function getMessageData() {
    const queryString =
      "stream=" +
      props.content["stream"] +
      "&sequenceNumber=" +
      props.content["sequenceNumber"];
    fetch("/api/messageData?" + queryString)
      .then((res) => res.json())
      .then((jsonData) => {
        // jsonData is a string for some reason
        console.log(jsonData);
      });
  }

  return <Button onClick={getMessageData}>View Data</Button>;
}

export { MsgModal };
