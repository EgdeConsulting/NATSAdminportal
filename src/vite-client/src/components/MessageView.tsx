import { PaginatedTable, ContentHider } from "./";

function MessageView(props: { messages: any[] }) {
  const columns = [
    {
      Header: "All Messages",
      columns: [
        {
          Header: "Timestamp",
          accessor: "messageTimestamp",
          appendChildren: "false",
        },
        {
          Header: "Subject",
          accessor: "messageSubject",
          appendChildren: "false",
        },
        {
          Header: "Acknowledgement",
          accessor: "messageAcknowledgement",
          appendChildren: "false",
        },
        {
          Header: "Headers",
          accessor: "messageHeaders",
          appendChildren: "true",
        },
        {
          Header: "Payload",
          accessor: "messagePayload",
          appendChildren: "true",
        },
      ],
    },
  ];

  return (
    <PaginatedTable columns={columns} data={props.messages}>
      <ContentHider content={""} />
    </PaginatedTable>
  );
}

export { MessageView };
