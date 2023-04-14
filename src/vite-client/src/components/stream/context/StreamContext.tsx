import { createContext, useState } from "react";

export interface IStream {
  name: string;
  subjects?: number;
  consumers?: number;
  messages?: number;
}

interface IPolicies {
  discard: string;
  retention: string;
}
export interface IStreamExtended {
  name: string;
  description: string;
  subjects: string[];
  consumers: string[];
  deleted: number;
  policies: IPolicies;
}

export type StreamContextType = {
  currentStream: IStream;
  changeCurrentStream: (stream: IStream) => void;
};

const DefaultStreamState = {
  currentStream: { name: "" },
  changeCurrentStream: () => {},
};

const StreamContext = createContext<StreamContextType>(DefaultStreamState);

function StreamContextProvider(props: {
  children: JSX.Element | JSX.Element[];
}) {
  const [currentStream, setCurrentStream] = useState<IStream>(
    DefaultStreamState.currentStream
  );
  function changeCurrentStream(newStream: IStream) {
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
