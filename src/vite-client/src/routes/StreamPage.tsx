import { Button } from "@chakra-ui/react";

function StreamPage() {
  function getStreams() {
    fetch("/StreamInfo")
      .then((res: any) => res.json())
      .then((data) => {
        console.log(data);
      });
  }

  return <Button onClick={getStreams}></Button>;
}

export { StreamPage };
