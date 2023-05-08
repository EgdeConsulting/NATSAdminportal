import { createContext, useState } from "react";

export interface StreamProps {
  name: string;
  subjects?: number;
  consumers?: number;
  messages?: number;
}

interface PoliciesProps {
  discard: string;
  retention: string;
}

export interface ConsumerProps {
  name: string;
  created: Date;
}

export interface StreamExtendedProps {
  name: string;
  description: string;
  subjects: string[];
  consumers: ConsumerProps[];
  deleted: number;
  policies: PoliciesProps;
}

export type StreamContextType = {
  currentStream: StreamProps;
  changeCurrentStream: (stream: StreamProps) => void;
};

const DefaultStreamState = {
  currentStream: { name: "" },
  changeCurrentStream: () => {},
};

const StreamContext = createContext<StreamContextType>(DefaultStreamState);

function StreamContextProvider(props: {
  children: JSX.Element | JSX.Element[];
}) {
  const [currentStream, setCurrentStream] = useState<StreamProps>(
    DefaultStreamState.currentStream
  );
  function changeCurrentStream(newStream: StreamProps) {
    setCurrentStream(newStream);
  }

  return (
    <StreamContext.Provider
      value={{
        currentStream: currentStream,
        changeCurrentStream: changeCurrentStream,
      }}
    >
      {props.children}
    </StreamContext.Provider>
  );
}

export { DefaultStreamState, StreamContext, StreamContextProvider };
